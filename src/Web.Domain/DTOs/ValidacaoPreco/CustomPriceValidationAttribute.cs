using System.ComponentModel.DataAnnotations;

namespace Web.Domain.ValidacaoPreco
{
    public class CustomPriceValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is decimal price)
            {
                if (price >= 0.01m && price <= 99999.99m)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}