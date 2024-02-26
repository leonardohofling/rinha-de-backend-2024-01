using System.ComponentModel.DataAnnotations;

namespace RinhaDeBackend.API.Validators
{
    public class TypeValidatorAttribute : ValidationAttribute
    {
        public TypeValidatorAttribute() : base("The {0} field must be 'c' or 'd'") { }
        public override bool IsValid(object? value)
        {
            return value is string && value != null && ((string)value == "c" || (string)value == "d");
        }
    }
}
