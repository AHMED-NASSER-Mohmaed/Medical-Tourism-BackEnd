using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Disbursement
{
    public class DisplayDisbursement
    {
        public int Id { get; set; }
        public DateOnly DisbursementDateMonth { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime GeneratedAt { get; set; } 
        public string PaymentMethod { get; set; } // Method of payment (e.g., cash, bank transfer)
       // public string AssetId { get; set; }
       

        public Collection<IDisplayDisbursementItems> DisbursementItems { get; set; } = new Collection<IDisplayDisbursementItems>();



    }
}
