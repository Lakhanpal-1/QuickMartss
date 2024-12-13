using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QuickMart.Data.Entities;

namespace QuickMart.Data.Repository.IRepository
{
    public interface IProductRepository
    {
        #region Product Retrieval Methods

        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<Product> GetProductByIdAsync(int productId);

        #endregion

        #region Product Creation and Update Methods

        Task<Product> CreateProductAsync(Product product, IFormFile? productImage);

        Task<Product> UpdateProductAsync(Product product);

        #endregion

        #region Product Deletion Method

        Task<bool> DeleteProductAsync(int productId);

        #endregion
    }
}
