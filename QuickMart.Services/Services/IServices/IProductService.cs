using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickMart.Data.DTO;

namespace QuickMart.Services.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();                // Retrieves a list of ProductDTOs
        Task<ProductDTO> GetProductByIdAsync(int productId);                // Retrieves a single ProductDTO by its ID
        Task<ProductDTO> CreateProductAsync(ProductDTO productDTO);         // Creates a new product from ProductDTO and returns ProductDTO
        Task<ProductDTO> UpdateProductAsync(int productId, ProductDTO productDTO); // Updates an existing product with the given ProductDTO
        Task<bool> DeleteProductAsync(int productId);                       // Deletes a product and returns a success flag
    }
}
