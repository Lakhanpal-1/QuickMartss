using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using QuickMart.Data.DTO;
using QuickMart.Data.Entities;
using QuickMart.Data.Repositories;
using QuickMart.Data.Repository.IRepository;
using QuickMart.Services.Services.IServices;

namespace QuickMart.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
            private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        #region Product Retrieval Methods

        // Step 1: Get all products
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int page, int pageSize, string sortBy, string sortOrder)
        {
            var products = await _productRepository.GetAllProductsAsync(page, pageSize, sortBy, sortOrder);

            // Map the products to ProductDTOs
            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return productDTOs;
        }



        // Step 2: Get product by ID
        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return null; // Or handle as necessary
            }

            var productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }

        #endregion

        #region Product Creation and Update Methods

        // Step 3: Create a new product
        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(productDTO.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Invalid category.");
            }

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                StockQuantity = productDTO.StockQuantity,
                IsActive = productDTO.IsActive,
                DiscountPrice = productDTO.DiscountPrice,
                CategoryId = productDTO.CategoryId, // Set category ID
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateProductAsync(product, productDTO.ProductImage);

            var createdProductDTO = new ProductDTO
            {
                ProductId = createdProduct.ProductId,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity,
                IsActive = createdProduct.IsActive,
                DiscountPrice = createdProduct.DiscountPrice,
                CategoryId = createdProduct.CategoryId,
                CategoryName = createdProduct.Category?.Name, // Include category name
                ImageUrl = createdProduct.ImageUrl
            };

            return createdProductDTO;
        }


        // Step 4: Update an existing product
        public async Task<ProductDTO> UpdateProductAsync(int productId, ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            product.ProductId = productId;

            var updatedProduct = await _productRepository.UpdateProductAsync(product);

            var updatedProductDTO = _mapper.Map<ProductDTO>(updatedProduct);

            return updatedProductDTO;
        }

        #endregion

        #region Product Deletion Methods

        // Step 5: Delete a product
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var result = await _productRepository.DeleteProductAsync(productId);
            return result;
        }

        #endregion
    }
}
