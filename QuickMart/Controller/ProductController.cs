using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickMart.Data.DTO;
using QuickMart.Services.Services.IServices;

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

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }

        /// <summary>
        /// Create a new product without image upload.
        /// </summary>
        /// <param name="productDTO">Product data including name, description, price, etc.</param>
        /// <returns>Returns the created product data.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
        {
            var createdProduct = await _productService.CreateProductAsync(productDTO);
            return CreatedAtAction(nameof(CreateProduct), createdProduct);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest("Invalid product data.");
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, productDTO);
            if (updatedProduct == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(updatedProduct);
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound("Product not found.");
            }

            return Ok("Product deleted successfully.");
        }
    }
}
