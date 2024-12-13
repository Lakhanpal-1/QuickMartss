using System;
using System.ComponentModel.DataAnnotations;

namespace QuickMart.Data.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]  // Enforcing a maximum length of 100 for the product name
        public string Name { get; set; }

        [StringLength(500)]  // Optional description with a maximum length of 500 characters
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        // Added IsActive property to the entity.
        public bool IsActive { get; set; } = true;  // Default is active if not provided

        // Added DiscountPrice property to the entity.
        public decimal? DiscountPrice { get; set; }  // Nullable because not every product may have a discount

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // New property to store the image URL
        public string? ImageUrl { get; set; }
    }
}
