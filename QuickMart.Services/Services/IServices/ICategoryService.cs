using QuickMart.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickMart.Services.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryDTO> UpdateCategoryAsync(int categoryId, CategoryDTO categoryDTO);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
