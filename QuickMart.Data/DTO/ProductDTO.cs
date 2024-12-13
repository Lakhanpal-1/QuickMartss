using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

public class ProductDTO
{
    [SwaggerSchema(ReadOnly = true)]
    public int ProductId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // IsActive property included in the DTO.
    public bool IsActive { get; set; }

    // DiscountPrice property included in the DTO.
    public decimal? DiscountPrice { get; set; }

    // Adding the image property to the DTO for accepting the image file.
    public IFormFile? ProductImage { get; set; }

    // Image URL that will be returned after saving the image to the file system.
    public string? ImageUrl { get; set; }
}
