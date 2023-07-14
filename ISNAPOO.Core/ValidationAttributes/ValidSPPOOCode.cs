using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ValidationAttributes
{
    public class ValidSPPOOCode : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            int result;
            int.TryParse(value.ToString(), out result);

            if (result != 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("В полето 'Код' може да въвеждате само числови стойности!");
            }
        }
    }
}
