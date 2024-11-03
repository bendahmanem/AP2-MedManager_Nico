using System.ComponentModel.DataAnnotations;

public class RequiredPatientAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int intValue && intValue != 0)
        {
            return ValidationResult.Success!;
        }
        return new ValidationResult(ErrorMessage ?? "Veuillez sélectionner un patient valide.");
    }
}
