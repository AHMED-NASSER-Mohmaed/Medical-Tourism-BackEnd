using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Disbursement
    {
        int Id { get; set; } 
        public DateOnly DisbursementDateMonth { get; set; } 
        public decimal TotalAmount { get; set; } 
        public DateTime GeneratedAt { get; set; } = DateTime.Now; 
        public string PaymentMethod { get; set; } // Method of payment (e.g., cash, bank transfer)

        public List<DisbursementItem> DisbursementItems { get; set; } = new List<DisbursementItem>(); // List of disbursement items
    
    }
}
