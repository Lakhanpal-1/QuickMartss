using Microsoft.AspNetCore.Mvc;
using QuickMart.Services.Services.IServices;
using QuickMart.Services.DTO;
using System.Threading.Tasks;
using QuickMart.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using System;
using QuickMart.Services.Services;

namespace QuickMart.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        #region Category CRUD Operations

        /// <summary>
        /// Creates a new category.
        /// </summary>
        [HttpPost("create")]
        [SwaggerResponse(201, "Category successfully created.", typeof(CategoryDTO))]
        [SwaggerResponse(400, "Invalid category data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await categoryService.CreateCategoryAsync(categoryDTO);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a category by ID.
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Category retrieved successfully.", typeof(CategoryDTO))]
        [SwaggerResponse(404, "Category not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound("Category not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        [HttpGet("all")]
        [SwaggerResponse(200, "List of categories retrieved successfully.", typeof(IEnumerable<CategoryDTO>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates category data by ID.
        /// </summary>
        [HttpPut("{id}")]
        [SwaggerResponse(200, "Category updated successfully.", typeof(CategoryDTO))]
        [SwaggerResponse(400, "Invalid category data.")]
        [SwaggerResponse(404, "Category not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null || id <= 0)
            {
                return BadRequest("Invalid category data.");
            }

            try
            {
                var updatedCategory = await categoryService.UpdateCategoryAsync(id, categoryDTO);
                if (updatedCategory == null)
                {
                    return NotFound("Category not found.");
                }
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a category by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Category deleted successfully.")]
        [SwaggerResponse(404, "Category not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound("Category not found.");
                }
                return Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
