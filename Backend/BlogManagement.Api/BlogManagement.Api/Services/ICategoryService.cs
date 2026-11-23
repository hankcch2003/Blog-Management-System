using System.Collections.Generic;
using System.Threading.Tasks;
using BlogManagement.Api.DTOs.Categories;

namespace BlogManagement.Api.Services
{
    public interface ICategoryService
    {
        // 取得所有分類
        Task<List<CategoryResponseDto>> GetAllAsync();

        // 依照 ID 取得單一分類
        Task<CategoryResponseDto?> GetByIdAsync(int id);

        // 新增分類
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);

        // 修改分類
        Task<CategoryResponseDto?> UpdateAsync(
            int id,
            UpdateCategoryDto dto
        );

        // 刪除分類
        Task<bool> DeleteAsync(int id);
    }
}