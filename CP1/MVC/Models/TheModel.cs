using System.ComponentModel.DataAnnotations;
namespace MVC.Models;

public class TheModel
{
    [Required(ErrorMessage = "The phrase is required.")]
    [StringLength(25, MinimumLength = 5, ErrorMessage = "The phrase must be between 5 and 25 characters.")]
    public string? Phrase { get; set; }

    public Dictionary<char, int> Counts { get; set; } = [];

    public string? Lower { get; set; }

    public string? Upper { get; set; }
}
