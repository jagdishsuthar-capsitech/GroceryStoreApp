using System.ComponentModel.DataAnnotations;

namespace GroceryApp.DTOs;

public class ProductDto
{
    [RegularExpression(@"^[a-fA-F0-9]{24}$", ErrorMessage = "Invalid product ID.")]
    public string? Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "99999")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}