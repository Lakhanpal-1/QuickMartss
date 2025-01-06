using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickMart.Data.DTO;
using QuickMart.Services.Services.IServices;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace QuickMart.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        #region Product Retrieval Methods

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, "Products retrieved successfully.", typeof(IEnumerable<ProductDTO>))]
        [SwaggerResponse(404, "No products found.")]
        [SwaggerResponse(500, "Internal server error.")]
       
        public async Task<IActionResult> GetAllProducts(int page = 1, int pageSize = 5, string sortBy = "Name", string sortOrder = "asc")
        {
            var products = await _productService.GetAllProductsAsync(page, pageSize, sortBy, sortOrder);

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Product retrieved successfully.", typeof(ProductDTO))]
        [SwaggerResponse(404, "Product not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(product);
        }

        #endregion

        #region Product Management Methods

        /// <summary>
        /// Creates a new product without image upload.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerResponse(201, "Product created successfully.", typeof(ProductDTO))]
        [SwaggerResponse(400, "Product image is required.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO productDTO)
        {
            if (productDTO.ProductImage == null)
            {
                return BadRequest("Product image is required.");
            }

            try
            {
                var createdProduct = await _productService.CreateProductAsync(productDTO);

                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerResponse(200, "Product updated successfully.", typeof(ProductDTO))]
        [SwaggerResponse(400, "Invalid product data.")]
        [SwaggerResponse(404, "Product not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest("Invalid product data.");
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, productDTO);
            if (updatedProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(updatedProduct);
        }

        #endregion

        #region Product Deletion Methods

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerResponse(200, "Product deleted successfully.")]
        [SwaggerResponse(404, "Product not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok($"Product with ID {id} deleted successfully.");
        }

        #endregion
    }
}
