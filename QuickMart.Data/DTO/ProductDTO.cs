using Swashbuckle.AspNetCore.Annotations;

namespace QuickMart.Data.DTO
{
    public class ProductDTO
    {
        // Making ProductId read-only in Swagger UI.
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
    }
}
