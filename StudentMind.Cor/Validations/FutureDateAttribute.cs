using System.ComponentModel.DataAnnotations;

namespace StudentMind.Core.Validations
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null; 
            }

            var date = (DateTimeOffset)value;
            if (date <= DateTimeOffset.UtcNow)
            {
                return new ValidationResult("Start time must be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}