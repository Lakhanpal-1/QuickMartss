using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickMart.Data.DTO;

namespace QuickMart.Services.Services.IServices
{
    public interface IProductService
    {
        #region Product Retrieval Methods
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int page, int pageSize, string sortBy,  string sortOrder);

        Task<ProductDTO> GetProductByIdAsync(int productId);

        #endregion

        #region Product Creation and Update Methods

        Task<ProductDTO> CreateProductAsync(ProductDTO productDTO);

        Task<ProductDTO> UpdateProductAsync(int productId, ProductDTO productDTO);

        #endregion

        #region Product Deletion Methods

        Task<bool> DeleteProductAsync(int productId);

        #endregion
    }
}
