using System.ComponentModel.DataAnnotations;

namespace Common.Validation
{
    public class RequireOneOfTwoFieldsAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public RequireOneOfTwoFieldsAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherValue = validationContext.ObjectType.GetProperty(_otherProperty)
                ?.GetValue(validationContext.ObjectInstance, null) as string;

            if (string.IsNullOrWhiteSpace(value as string) && string.IsNullOrWhiteSpace(otherValue))
            {
                return new ValidationResult($"Either {validationContext.MemberName} or {_otherProperty} must be provided.");
            }

            return ValidationResult.Success;
        }
    }
}
