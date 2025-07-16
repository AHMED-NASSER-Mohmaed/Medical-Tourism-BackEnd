using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Trainers;
using Elagy.Core.DTOs.MlPrediction;
using Elagy.Core.IServices;
using Microsoft.ML.Data;
namespace Elagy.BL.Services
{
   public class MlTrainingService : IMLTrainingService
    {
        private readonly  IUnitOfWork _unitOfWork;
       
        public MlTrainingService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> TrainModel()
        {
            try
            {
                var mlContext = new MLContext();

                var ratingsFromDb = await _unitOfWork.MLAppointments.GetAlBookingPatient();

                Console.WriteLine("get data successfully");
                // IMPORTANT: remove items with missing required fields
                var cleaned = ratingsFromDb
                    .Where(r => r.UserId != null
                             && r.HospitalAssetId != null
                             && r.Label != null)
                    .Select(r => new HospitalRatingDto
                    {
                        UserId = r.UserId,
                        HospitalAssetId = r.HospitalAssetId,
                        Label = r.Label,

                        Address = r.Address,
                        City = r.City,
                        GovernorateId = r.GovernorateId,
                        Age = r.Age,
                        BloodGroup = r.BloodGroup,
                        Height = r.Height,
                        Weight = r.Weight,
                        Appointementprice = r.Appointementprice,
                        SpecialtyScheduleId = r.SpecialtyScheduleId,
                        HospitalSpecialtyId = r.HospitalSpecialtyId,
                        HotelAssetId = r.HotelAssetId,
                        CarRentalAssetId=r.CarRentalAssetId
                    })
                    .ToList();
                Console.WriteLine("after clean");

                if (!cleaned.Any())
                    throw new Exception("No valid data to train the model.");

                var data = mlContext.Data.LoadFromEnumerable(cleaned);

                Console.WriteLine("after load");

                // Step 2: split
                var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
                Console.WriteLine("after split");

                // Step 3: pipeline
                var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("UserIdEncoded", nameof(HospitalRatingDto.UserId))
    .Append(mlContext.Transforms.Categorical.OneHotEncoding("HospitalAssetIdEncoded", nameof(HospitalRatingDto.HospitalAssetId)))
    .Append(mlContext.Transforms.Categorical.OneHotEncoding("BloodGroupEncoded", nameof(HospitalRatingDto.BloodGroup)))
    .Append(mlContext.Transforms.Categorical.OneHotEncoding("CityEncoded", nameof(HospitalRatingDto.City)))
    .Append(mlContext.Transforms.Categorical.OneHotEncoding("AddressEncoded", nameof(HospitalRatingDto.Address)))
    .Append(mlContext.Transforms.Categorical.OneHotEncoding("HotelAssetIdEncoded", nameof(HospitalRatingDto.HotelAssetId)))
    .Append(mlContext.Transforms.Categorical.OneHotEncoding("CarRentalAssetIdEncoded", nameof(HospitalRatingDto.CarRentalAssetId)))

    // ✅ نحول الأعمدة int إلى float
    .Append(mlContext.Transforms.Conversion.ConvertType(
        new[] {
            new InputOutputColumnPair("GovernorateIdFloat", nameof(HospitalRatingDto.GovernorateId)),
            new InputOutputColumnPair("SpecialtyScheduleIdFloat", nameof(HospitalRatingDto.SpecialtyScheduleId)),
            new InputOutputColumnPair("HospitalSpecialtyIdFloat", nameof(HospitalRatingDto.HospitalSpecialtyId))
        },
        outputKind: DataKind.Single))
    .Append(mlContext.Transforms.Concatenate("Features",
        "UserIdEncoded", "HospitalAssetIdEncoded", "BloodGroupEncoded", "CityEncoded", "AddressEncoded",
        "GovernorateIdFloat", "Age", "Height", "Weight", "Appointementprice", "SpecialtyScheduleIdFloat", "HospitalSpecialtyIdFloat", "HotelAssetIdEncoded", "CarRentalAssetIdEncoded"))
    .Append(mlContext.Regression.Trainers.FastTree(
        labelColumnName: nameof(HospitalRatingDto.Label),
        featureColumnName: "Features"));
                Console.WriteLine("after Pipline");
                // Step 4: train
                var model = pipeline.Fit(split.TrainSet);
                Console.WriteLine("after train");

                // Step 5: save
                mlContext.Model.Save(model, split.TrainSet.Schema, "MLModel.zip");

                Console.WriteLine($"✅ Model trained & saved to: ");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        

    }
}
