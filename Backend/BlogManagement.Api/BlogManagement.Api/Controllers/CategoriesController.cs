using BlogManagement.Api.DTOs.Categories;
using BlogManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories
        // 取得所有分類
        [HttpGet]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        // GET: api/categories/{id}
        // 依照 ID 取得單一分類
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = $"找不到 ID 為 {id} 的分類。"
                });
            }

            return Ok(category);
        }

        // POST: api/categories
        // 新增分類
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> Create(
            [FromBody] CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category
            );
        }

        // PUT: api/categories/{id}
        // 修改分類
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> Update(
            int id,
            [FromBody] UpdateCategoryDto dto)
        {
            var category = await _categoryService.UpdateAsync(id, dto);

            if (category == null)
            {
                return NotFound(new
                {
                    message = $"找不到 ID 為 {id} 的分類。"
                });
            }

            return Ok(category);
        }

        // DELETE: api/categories/{id}
        // 刪除分類
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _categoryService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = $"找不到 ID 為 {id} 的分類。"
                    });
                }

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "此分類仍有文章使用，無法刪除。請先處理該分類底下的文章。"
                });
            }
        }
    }
}