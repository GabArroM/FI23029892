using System.ComponentModel.DataAnnotations;
namespace Practica2.Models;

public class Multiplo2 : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var str = value as string;
        if (!string.IsNullOrEmpty(str) && str.Length % 2 != 0)
        {
            return new ValidationResult(ErrorMessage ?? "La longitud debe ser múltiplo de 2");
        }
        return ValidationResult.Success;
    }
}
