using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickMart.Data.Entities;

namespace QuickMart.Data.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();                // Retrieves a list of Product entities
        Task<Product> GetProductByIdAsync(int productId);                // Retrieves a single Product entity by its ID
        Task<Product> CreateProductAsync(Product product);               // Creates a new Product entity and returns it
        Task<Product> UpdateProductAsync(Product product);               // Updates an existing Product entity and returns it
        Task<bool> DeleteProductAsync(int productId);                    // Deletes a product entity and returns success flag
    }
}
