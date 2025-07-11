using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Package
{
    public class PackageWithDetailDTO
    {
        /*        
                 
        hospital appointment
        doctorName - doctorSpecialty - hospitalName - hospitalAddress- appointmentDate- appointmentTime- price
        -----------
        hotel
        hotelName - hotelAddress - roomType -roomView -  checkInDate - checkOutDate - numberOfNights - price 
        --------
        car
        carModel - rentalCompany - pickupDate - dropoffDate - price*/
        public string DoctorName { get; set; }
        public string DoctorImageUrl { get; set; }
        public string DoctorSpecialtyName { get; set; }
        public string HospitalName { get; set; }
        public int HospitalGovId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentExistingTime { get; set; }
        public decimal AppointmentPrice { get; set; }
        public string HotelName { get; set; }
        public int HotelGovId { get; set; }
        public RoomCategory RoomType { get; set; }
        public string RoomImageUrl { get; set; }
        public ViewType RoomView { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public decimal HotelPrice { get; set; }
        public string CarModel { get; set; }
        public string CarImageUrl { get; set; }
        public string rentalCompanyName { get; set; }
        public DateOnly  CarStartingDate { get; set; }
        public DateOnly  EndingStartingDate { get; set; }
        public decimal CarPrice { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string DriverImageUrl { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return AppointmentPrice + HotelPrice + CarPrice;
            }

        }

    }
}
