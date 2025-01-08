using QuickMart.Data.DTO;
using QuickMart.Data.Entities;
using QuickMart.Data.Repositories;
using QuickMart.Services.Services.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickMart.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            }).ToList();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) return null;

            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = new Category
            {
                Name = categoryDTO.Name
            };
            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);

            return new CategoryDTO
            {
                CategoryId = createdCategory.CategoryId,
                Name = createdCategory.Name
            };
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int categoryId, CategoryDTO categoryDTO)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) return null;

            category.Name = categoryDTO.Name;
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);

            return new CategoryDTO
            {
                CategoryId = updatedCategory.CategoryId,
                Name = updatedCategory.Name
            };
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            return await _categoryRepository.DeleteCategoryAsync(categoryId);
        }
    }
}
