using System.ComponentModel.DataAnnotations;

namespace GroceryApp.DTOs;

public class StoreDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one product is required.")]
    public List<ProductDto> Inventory { get; set; } = new();
}