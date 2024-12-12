using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using QuickMart.Data.DTO;
using QuickMart.Data.Entities;
using QuickMart.Data.Repository.IRepository;
using QuickMart.Services.Services.IServices;

namespace QuickMart.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                StockQuantity = productDTO.StockQuantity,
                IsActive = productDTO.IsActive,   // Mapping IsActive to the entity
                DiscountPrice = productDTO.DiscountPrice, // Mapping DiscountPrice to the entity
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateProductAsync(product);

            return new ProductDTO
            {
                ProductId = createdProduct.ProductId,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity,
                IsActive = createdProduct.IsActive, // Mapping IsActive back to DTO
                DiscountPrice = createdProduct.DiscountPrice // Mapping DiscountPrice back to DTO
            };
        }

        public async Task<ProductDTO> UpdateProductAsync(int productId, ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            product.ProductId = productId; // Ensure the ID is set for update
            var updatedProduct = await _productRepository.UpdateProductAsync(product);
            return _mapper.Map<ProductDTO>(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            return await _productRepository.DeleteProductAsync(productId);
        }
    }
}
