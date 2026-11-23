using BlogManagement.Api.DTOs.Posts;

namespace BlogManagement.Api.Services
{
    public interface IPostService
    {
        // 取得所有文章
        Task<List<PostResponseDto>> GetAllAsync();

        // 依照 ID 取得單一文章
        Task<PostResponseDto?> GetByIdAsync(int id);

        // 新增文章
        Task<PostResponseDto> CreateAsync(CreatePostDto dto);

        // 修改文章
        Task<PostResponseDto?> UpdateAsync(
            int id,
            UpdatePostDto dto
        );

        // 刪除文章
        Task<bool> DeleteAsync(int id);
    }
}