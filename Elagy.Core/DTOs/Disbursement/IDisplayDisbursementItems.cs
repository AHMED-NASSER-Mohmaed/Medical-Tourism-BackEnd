using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Disbursement
{
   public interface IDisplayDisbursementItems
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; } // Amount of the disbursement item
      
    }
}
