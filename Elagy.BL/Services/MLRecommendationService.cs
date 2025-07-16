using AutoMapper;
using Elagy.Core.DTOs.MlPrediction;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class MLRecommendationService : IMLRecommendtionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<HospitalRatingDto, PredictionDto> _predictionEngine;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MLRecommendationService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mlContext = new MLContext();

            var modelFullPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModel.zip");

            if (!File.Exists(modelFullPath))
                throw new FileNotFoundException($"Model file not found at: {modelFullPath}");

            using var stream = new FileStream(modelFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _model = _mlContext.Model.Load(stream, out _);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<HospitalRatingDto, PredictionDto>(_model);
        }

         IQueryable<ServiceProvider> ApplyServiceProviderFilters(IQueryable<ServiceProvider> query, string? searchQuery, Status? userStatus)
        {
            // Ensure ServiceAsset is included for filtering on its properties, and for later mapping
            query = query.Include(sp => sp.ServiceAsset);

            // --- 1. Apply UserStatus Filter (Independent of search query) ---
            if (userStatus.HasValue)
            {
                query = query.Where(sp => sp.Status == userStatus.Value);
            }

            // --- 2. Apply Search Term Filter ---
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string lowerSearchQuery = searchQuery.Trim().ToLower(); // Trim any whitespace


                query = query.Where(sp =>
                                        sp.Email.ToLower().Contains(lowerSearchQuery) ||
                                        sp.FirstName.ToLower().Contains(lowerSearchQuery) ||
                                        sp.LastName.ToLower().Contains(lowerSearchQuery) ||
                                        sp.Phone.ToLower().Contains(lowerSearchQuery) ||
                                        sp.ServiceAsset.Name.ToLower().Contains(lowerSearchQuery)
                                   );
            }


            return query;
        }
        public async Task<PagedResponseDto<HospitalProviderProfileDto>> RecommendedHospitalForUser(PaginationParameters paginationParameters, string userId)
        {
            try
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(
            userId,
            new Expression<Func<Patient, object>>[] { p => p.Governorate, p => p.Governorate.Country }
        );
                if (patient == null)
                    throw new Exception($"Patient with id {userId} not found.");

               IQueryable<ServiceProvider> query = _unitOfWork.ServiceProviders
                                                .AsQueryable()
                                                .OfType<ServiceProvider>().Where(s=>s.Status==Status.Active);

            query = query.Include(sp => sp.Governorate)
                .ThenInclude(g => g.Country);

            query = query.Include(sp => sp.ServiceAsset).ThenInclude(asset => ((HospitalAsset)asset).HospitalAssetImages).Include(s => s.ServiceAsset)
                .ThenInclude(a => a.Governate)
                .ThenInclude(g => g.Country)
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.Hospital);


            if (paginationParameters.SpecialtyId.HasValue)
            {
                query = query.Where(sp =>
                   ((HospitalAsset)sp.ServiceAsset)
                   .HospitalSpecialties.Any(hs => hs.SpecialtyId == paginationParameters.SpecialtyId.Value)
               );
            }
            
            if (paginationParameters.FilterGovernorateId.HasValue)
            {
                query = query.Where(sp => sp.ServiceAsset.GovernateId == paginationParameters.FilterGovernorateId.Value);
            }

            query = ApplyServiceProviderFilters(query, paginationParameters.SearchTerm, paginationParameters.UserStatus);

                var hospitals = await query.ToListAsync();
                // 5. Score each hospital with ML model
                var scoredHospitals = hospitals
                    .Select(h =>
                    {
                        var input = new HospitalRatingDto
                        {
                            UserId = userId,
                            HospitalAssetId = h.Id, // Assuming h.Id is string

                            Label = 0f, // dummy during prediction

                            Address = patient.Address,
                            City = patient.City,
                            GovernorateId = patient.GovernorateId ?? 1,
                            Age = CalculateAge(patient.DateOfBirth?? new DateTime(1/11/1990)),
                            BloodGroup = patient.BloodGroup,
                            Height = patient.Height ?? 1f,
                            Weight = patient.Weight ?? 1f,
                            SpecialtyScheduleId=1,
                            Appointementprice = 1,
                            HospitalSpecialtyId = 1,

                          
                        };

Console.WriteLine($"UserId-{input.UserId}-{input.HospitalAssetId}-{input.Label}-{input.Address}-{input.City}-{input.GovernorateId}-{input.Age}-{input.BloodGroup}-{input.Height}-{input.Weight}-{input.SpecialtyScheduleId}-{input.Appointementprice}-{input.HospitalSpecialtyId}");
                        var prediction = _predictionEngine.Predict(input);
                        Console.WriteLine($"score -{prediction.Score}");
                        return new
                        {
                            Hospital = h,
                            Score = prediction.Score
                        };
                        
                    })
                    .OrderByDescending(x => x.Score)
                    .ToList();


                // 6. Apply paging
                var totalCount = scoredHospitals.Count;

                var paged = scoredHospitals
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .Select(x => x.Hospital)
                    .ToList();

                // 7. Map to DTOs
                var dtoList = _mapper.Map<List<HospitalProviderProfileDto>>(paged);

                // 8. Wrap in paged response
                return new PagedResponseDto<HospitalProviderProfileDto>(
                    dtoList,
                    totalCount,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedResponseDto<HotelProviderProfileDto>> RecommendetionHotelsForUser(PaginationParameters paginationParameters, string userId)
        {
            try
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(userId,new Expression<Func<Patient, object>>[] { p => p.Governorate, p => p.Governorate.Country });

                if (patient == null)
                    throw new Exception($"Patient with id {userId} not found.");

                var query = _unitOfWork.ServiceProviders.AsQueryable().OfType<ServiceProvider>().Where(s => s.Status == Status.Active);


                query = query.Include(sp => sp.ServiceAsset).ThenInclude(asset => ((HotelAsset)asset).HotelAssetImages); ;

                query = query.Include(sp => sp.Governorate)
                 .ThenInclude(g => g.Country);

                query = query.Include(sp => sp.ServiceAsset)
                .ThenInclude(a => a.Governate)
                .ThenInclude(g => g.Country);


                query = query.Where(sp => sp.ServiceAsset.AssetType == AssetType.Hotel);

                if (paginationParameters.FilterGovernorateId.HasValue)
                {
                    query = query.Where(s => s.ServiceAsset.GovernateId == paginationParameters.FilterGovernorateId.Value);
                }
                query = ApplyServiceProviderFilters(query, paginationParameters.SearchTerm, userStatus: paginationParameters.UserStatus);

                var hotels = await query.ToListAsync();

                var scoredHospitals = hotels
                    .Select(h =>
                    {
                        var input = new HospitalRatingDto
                        {
                            UserId = userId,
                            HospitalAssetId = h.Id, // Assuming h.Id is string

                            Label = 0f, // dummy during prediction

                            Address = patient.Address,
                            City = patient.City,
                            GovernorateId = patient.GovernorateId ?? 1,
                            Age = CalculateAge(patient.DateOfBirth ?? new DateTime(1 / 11 / 1990)),
                            BloodGroup = patient.BloodGroup,
                            Height = patient.Height ?? 1f,
                            Weight = patient.Weight ?? 1f,
                            SpecialtyScheduleId = 1,
                            Appointementprice = 1,
                            HospitalSpecialtyId = 1,
                            HotelAssetId = h.Id,
                            CarRentalAssetId = "no",




                        };

                        Console.WriteLine($"UserId-{input.UserId}-{input.HospitalAssetId}-{input.Label}-{input.Address}-{input.City}-{input.GovernorateId}-{input.Age}-{input.BloodGroup}-{input.Height}-{input.Weight}-{input.SpecialtyScheduleId}-{input.Appointementprice}-{input.HospitalSpecialtyId}");
                        var prediction = _predictionEngine.Predict(input);
                        Console.WriteLine($"score -{prediction.Score}");
                        return new
                        {
                            Hospital = h,
                            Score = prediction.Score
                        };

                    })
                    .OrderByDescending(x => x.Score)
                    .ToList();


                // 6. Apply paging
                var totalCount = scoredHospitals.Count;

                var paged = scoredHospitals
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .Select(x => x.Hospital)
                    .ToList();

                // 7. Map to DTOs
                var dtoList = _mapper.Map<List<HotelProviderProfileDto>>(paged);

                // 8. Wrap in paged response
                return new PagedResponseDto<HotelProviderProfileDto>(
                    dtoList,
                    totalCount,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public int CalculateAge(DateTime dateOfBirth)
        {
            if (dateOfBirth == null) return 0;
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public async Task<PagedResponseDto<CarRentalProviderProfileDto>> RecommendetionCarRentalForUser(PaginationParameters paginationParameters, string userId)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(userId, new Expression<Func<Patient, object>>[] { p => p.Governorate, p => p.Governorate.Country });

            if (patient == null)
                throw new Exception($"Patient with id {userId} not found.");

            IQueryable<ServiceProvider> query = _unitOfWork.ServiceProviders
                .AsQueryable()
                .OfType<ServiceProvider>().Where(s => s.Status == Status.Active); // Ensures we're working with ServiceProvider entities

            // 2. Apply the fixed filter for CarRental assets
            //    Include ServiceAsset first, as it's needed for this filter (and likely for mapping).
            query = query
                .Include(sp => sp.ServiceAsset)
                .ThenInclude(asset => ((CarRentalAsset)asset).CarRentalAssetImages).Include(s => s.ServiceAsset).ThenInclude(a => a.Governate)
                .ThenInclude(g => g.Country)  // Eager load ServiceAsset if needed for AssetType filter and mapping
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.CarRental); // Filter for CarRental assets


            // 3. Apply additional dynamic filters (searchQuery, userStatus) using the helper function
            query = ApplyServiceProviderFilters(query, paginationParameters.SearchTerm, paginationParameters.UserStatus);

            var CarsRental = await query.ToListAsync();

            var scoredHospitals = CarsRental
                .Select(h =>
                {
                    var input = new HospitalRatingDto
                    {
                        UserId = userId,
                        HospitalAssetId = h.Id, // Assuming h.Id is string

                        Label = 0f, // dummy during prediction

                        Address = patient.Address,
                        City = patient.City,
                        GovernorateId = patient.GovernorateId ?? 1,
                        Age = CalculateAge(patient.DateOfBirth ?? new DateTime(1 / 11 / 1990)),
                        BloodGroup = patient.BloodGroup,
                        Height = patient.Height ?? 1f,
                        Weight = patient.Weight ?? 1f,
                        SpecialtyScheduleId = 1,
                        Appointementprice = 1,
                        HospitalSpecialtyId = 1,
                        HotelAssetId = "no",
                        CarRentalAssetId = h.Id,




                    };

                    Console.WriteLine($"CArUserId-{input.UserId}-{input.HospitalAssetId}-{input.Label}-{input.Address}-{input.City}-{input.GovernorateId}-{input.Age}-{input.BloodGroup}-{input.Height}-{input.Weight}-{input.SpecialtyScheduleId}-{input.Appointementprice}-{input.HospitalSpecialtyId}");
                    var prediction = _predictionEngine.Predict(input);
                    Console.WriteLine($"score -{prediction.Score}");
                    return new
                    {
                        Hospital = h,
                        Score = prediction.Score
                    };

                })
                .OrderByDescending(x => x.Score)
                .ToList();


            // 6. Apply paging
            var totalCount = scoredHospitals.Count;

            var paged = scoredHospitals
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .Select(x => x.Hospital)
                .ToList();

            // 7. Map to DTOs
            var dtoList = _mapper.Map<List<CarRentalProviderProfileDto>>(paged);

            // 8. Wrap in paged response
            return new PagedResponseDto<CarRentalProviderProfileDto>(
                dtoList,
                totalCount,
                paginationParameters.PageNumber,
                paginationParameters.PageSize
            );

        }
    }
}
