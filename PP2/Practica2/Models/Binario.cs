using System.ComponentModel.DataAnnotations;

namespace Practica2.Models;

public class Binario
{
    [Required(ErrorMessage = "Debe ingresar un valor para A.")]
    [RegularExpression("^[01]+$", ErrorMessage = "A solo puede contener 0 y 1.")]
    [StringLength(8, MinimumLength = 2, ErrorMessage = "A debe tener entre 2 y 8 caracteres.")]
    [Multiplo2(ErrorMessage = "A debe tener longitud múltiplo de 2.")]
    public string? A { get; set; }

    [Required(ErrorMessage = "Debe ingresar un valor para B.")]
    [RegularExpression("^[01]+$", ErrorMessage = "B solo puede contener 0 y 1.")]
    [StringLength(8, MinimumLength = 2, ErrorMessage = "B debe tener entre 2 y 8 caracteres.")]
    [Multiplo2(ErrorMessage = "A debe tener longitud múltiplo de 2.")]
    public string? B { get; set; }
}
