using Elagy.Core.DTOs.Disbursement;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class DisbursementRepository : GenericRepository<Disbursement> , IDisbursementRepository
    {
        public DisbursementRepository(ApplicationDbContext _context):base(_context)
        { }
        
        public async Task<IQueryable<Disbursement>> GetAllHospitalDisbursement(string AssetId)
        {
            Console.WriteLine("inside Repo");

            return _dbSet
                        .Where(d => d.AssetId == AssetId)
                        .Include(d => d.Asset) // Include the Asset navigation property
                        .Select(d => new Disbursement
                        {
                            Id = d.Id,
                            DisbursementDateMonth = d.DisbursementDateMonth,
                            TotalAmount = d.TotalAmount,
                            GeneratedAt = d.GeneratedAt,
                            PaymentMethod = d.PaymentMethod,
                            AssetId = d.AssetId,
                            Asset = d.Asset, // Include the Asset object
                        });

        }

       

        public async Task<DisbursementHospitalDTO> GetHospitalDisbursementById(int disbursementId)
        {
            var disbursement = await _dbSet
                                .Include(d => d.DisbursementItems)
                                    .ThenInclude(di => di.Appointment)
                                        .ThenInclude(a => ((SpecialtyAppointment)a).SpecialtySchedule)
                                            .ThenInclude(s => s.Doctor)
                                .Include(d => d.DisbursementItems)
                                    .ThenInclude(di => di.Appointment)
                                        .ThenInclude(a => ((SpecialtyAppointment)a).SpecialtySchedule)
                                            .ThenInclude(s => s.HospitalSpecialty)
                                                .ThenInclude(hs => hs.Specialty)
                                .FirstOrDefaultAsync(d => d.Id == disbursementId);

            if (disbursement == null) return null;

            // map in-memory:
            var dto = new DisbursementHospitalDTO
            {
                Id = disbursement.Id,
                DisbursementDateMonth = disbursement.DisbursementDateMonth,
                GeneratedAt = disbursement.GeneratedAt,
                TotalAmount = disbursement.TotalAmount,
                DisbursementItems = disbursement.DisbursementItems.Select(di => new DisbursementItemDto
                {
                    Id = di.Id,
                    Appointment = di.Appointment.Type == AppointmentType.Specialty && di.Appointment is SpecialtyAppointment sa
                        ? new AppointmentDto
                        {
                            Id = sa.Id,
                            price = sa.price,
                            Status = (int)sa.Status,
                            Type = (int)sa.Type,
                            SpecialtySchedule = sa.SpecialtySchedule != null
                                ? new SpecialtyScheduleDto
                                {
                                    Id = sa.SpecialtySchedule.Id,
                                    
                                    Specialty = sa.SpecialtySchedule.HospitalSpecialty?.Specialty?.Name, // <-- Specialty name
                                    Doctor = new DoctorDto
                                    {
                                        Id = sa.SpecialtySchedule.Doctor.Id,
                                        Name = sa.SpecialtySchedule.Doctor.FirstName + " " + sa.SpecialtySchedule.Doctor.LastName
                                    }
                                }
                                : null
                        }
                        : null
                }).ToList()
            };
            return dto;


        }
        public Task<IQueryable<Disbursement>> GetAllHotelDisbursement(string AssetId)
        {
            throw new NotImplementedException();
        }

        public Task<Disbursement> GetHotelDisbursementById(int disbursementId)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Disbursement>> GetAllCarRentalDisbursement(string AssetId)
        {
            throw new NotImplementedException();
        }

        public Task<Disbursement> GetCarRentalDisbursementById(int disbursementId)
        {
            throw new NotImplementedException();
        }
    }
}
