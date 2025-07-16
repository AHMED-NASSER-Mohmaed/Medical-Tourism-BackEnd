using Elagy.Core.DTOs.MlPrediction;
using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class MLRepository:  IMLRepository
    {
        private readonly ApplicationDbContext _context;
        public MLRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<List<HospitalRatingDto>> GetAlBookingPatient()
        {
            var dataset = new List<HospitalRatingDto>();

            // Step 1️⃣: get all real booked SpecialtyAppointments (positives)
            var positiveDtos = await _context.appointments
     .Include(a => a.Package).ThenInclude(pkg => pkg.Patient)
     .Include(a => ((SpecialtyAppointment)a).SpecialtySchedule).ThenInclude(s => s.HospitalSpecialty)
     .Include(a => ((RoomAppointment)a).RoomSchedule)
     .Include(a => ((RoomAppointment)a).Room)
     .Include(a => ((CarRentalAppointment)a).CarSchedule)
     .Select(a => new HospitalRatingDto
     {
         UserId = a.Package.PatientId,
         Address = a.Package.Patient.Address,
         City = a.Package.Patient.City,
         GovernorateId = a.Package.Patient.GovernorateId ?? 0,
         Age = CalculateAge(a.Package.Patient.DateOfBirth ?? new DateTime()),
         BloodGroup = a.Package.Patient.BloodGroup,
         Height = a.Package.Patient.Height ?? 175,
         Weight = a.Package.Patient.Weight ?? 75,
         Appointementprice = (float)(a.price),
         SpecialtyScheduleId = a is SpecialtyAppointment ? ((SpecialtyAppointment)a).SpecialtyScheduleId : 0,
         HospitalAssetId = a is SpecialtyAppointment ?
             ((SpecialtyAppointment)a).SpecialtySchedule.HospitalSpecialty.HospitalAssetId : "",
         HotelAssetId = a is RoomAppointment ?
             ((RoomAppointment)a).RoomSchedule.Room.HotelAssetId : "", // Assuming RoomSchedule has HotelAssetId
         CarRentalAssetId = a is CarRentalAppointment ?
             ((CarRentalAppointment)a).CarSchedule.Car.CarRentalAssetId : "", // Assuming CarSchedule has CarRentalAssetId
         HospitalSpecialtyId = a is SpecialtyAppointment ?
             ((SpecialtyAppointment)a).SpecialtySchedule.HospitalSpecialtyId : 0,
         Label = 1
     }).ToListAsync();

            dataset.AddRange(positiveDtos);

            // Step 2️⃣: get all specialty schedules (candidate negative items)
            var allSchedules = await _context.Schedules
                .Include(s => s.HospitalSpecialty)
                .ToListAsync();

            // Step 3️⃣: build negative samples per user
            var groupedByUser = positiveDtos.GroupBy(dto => dto.UserId);

            foreach (var userGroup in groupedByUser)
            {

                Console.WriteLine($"{userGroup.Key}- dob{userGroup.FirstOrDefault().Age}");
                var userId = userGroup.Key;

                var bookedHospitalIds = userGroup
                    .Select(dto => dto.HospitalAssetId)
                    .ToHashSet();

                // candidate negatives: schedules this user *didn’t* book
                var negativeSchedules = allSchedules
                    .Where(s => !bookedHospitalIds.Contains(s.HospitalSpecialty.HospitalAssetId))
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(100) // choose e.g. 10 negatives per user
                    .ToList();

                foreach (var schedule in negativeSchedules)
                {
                    dataset.Add(new HospitalRatingDto
                    {
                        UserId = userId,
                        Address = null, // or copy from userGroup.First() if you want same info
                        City = null,
                        GovernorateId = 0,
                        Age =userGroup.FirstOrDefault().Age,
                        BloodGroup = userGroup.FirstOrDefault().BloodGroup,
                        Height = userGroup.FirstOrDefault().Height,
                        Weight = userGroup.FirstOrDefault().Weight,

                        Appointementprice = (float)(schedule.Price),
                        SpecialtyScheduleId = schedule.Id,
                        HospitalAssetId = schedule.HospitalSpecialty.HospitalAssetId,
                        HospitalSpecialtyId = schedule.HospitalSpecialtyId,
                        Label = 0
                    });
                }
            }

            return dataset;
        }
        static int CalculateAge(DateTime dateOfBirth)
        {
            if(dateOfBirth==null)return 0;
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
