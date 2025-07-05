using Elagy.Core.DTOs.Booking;
using Elagy.Core.DTOs.Package;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class PackageService : IPackgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
         

        public async Task<Package> CreatePackage(string PatientId)
        {
            if (string.IsNullOrEmpty(PatientId))
            {
                throw new ArgumentException("Patient ID cannot be null or empty.", nameof(PatientId));
            }

            var createdPackage = new Package
            {
                PatientId = PatientId,
                CreatedAt = DateTime.UtcNow,
                //Status = BookingStatus.PendingPayment,
                Status = BookingStatus.Confirmed,

            };

            await _unitOfWork.Packages.AddAsync(createdPackage);


            return createdPackage;
        }
        


    }
}
