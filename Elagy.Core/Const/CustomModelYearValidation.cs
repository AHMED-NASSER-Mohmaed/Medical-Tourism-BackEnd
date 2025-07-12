using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Const
{
    public class CustomModelYearValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int modelYear = (int)value;
            if (modelYear > DateTime.Now.Year)
            {
                return new ValidationResult("Model year cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
