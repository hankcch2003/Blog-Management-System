using BlogManagement.Api.Data;
using BlogManagement.Api.DTOs.Posts;
using BlogManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Api.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 取得所有文章
        public async Task<List<PostResponseDto>> GetAllAsync()
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Select(post => new PostResponseDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    IsPublished = post.IsPublished,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    PublishedAt = post.PublishedAt,
                    CategoryId = post.CategoryId,
                    CategoryName = post.Category!.Name
                })
                .ToListAsync();
        }

        // 依照 ID 取得單一文章
        public async Task<PostResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Where(post => post.Id == id)
                .Select(post => new PostResponseDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    IsPublished = post.IsPublished,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    PublishedAt = post.PublishedAt,
                    CategoryId = post.CategoryId,
                    CategoryName = post.Category!.Name
                })
                .FirstOrDefaultAsync();
        }

        // 新增文章
        public async Task<PostResponseDto> CreateAsync(
            CreatePostDto dto)
        {
            // 確認分類是否存在
            var categoryExists = await _context.Categories
                .AnyAsync(category => category.Id == dto.CategoryId);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "指定的文章分類不存在。"
                );
            }

            // 建立文章 Entity
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Summary = dto.Summary,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                PublishedAt = dto.IsPublished
                    ? DateTime.UtcNow
                    : null,
                CategoryId = dto.CategoryId
            };

            // 新增文章
            _context.Posts.Add(post);

            // 儲存資料庫
            await _context.SaveChangesAsync();

            // 取得分類名稱
            var categoryName = await _context.Categories
                .Where(category => category.Id == post.CategoryId)
                .Select(category => category.Name)
                .FirstAsync();

            // 回傳文章 DTO
            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                IsPublished = post.IsPublished,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                PublishedAt = post.PublishedAt,
                CategoryId = post.CategoryId,
                CategoryName = categoryName
            };
        }

        // 修改文章
        public async Task<PostResponseDto?> UpdateAsync(
            int id,
            UpdatePostDto dto)
        {
            // 查詢文章，保留 EF Core 追蹤
            var post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id == id);

            // 找不到文章
            if (post == null)
            {
                return null;
            }

            // 確認新分類是否存在
            var categoryExists = await _context.Categories
                .AnyAsync(category => category.Id == dto.CategoryId);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "指定的文章分類不存在。"
                );
            }

            // 保存修改前的發布狀態
            var wasPublished = post.IsPublished;
            var previousPublishedAt = post.PublishedAt;

            // 更新文章基本資料
            post.Title = dto.Title;
            post.Content = dto.Content;
            post.Summary = dto.Summary;
            post.CategoryId = dto.CategoryId;
            post.IsPublished = dto.IsPublished;

            // 更新發布時間
            if (dto.IsPublished)
            {
                post.PublishedAt = wasPublished
                    ? previousPublishedAt ?? DateTime.UtcNow
                    : DateTime.UtcNow;
            }
            else
            {
                post.PublishedAt = null;
            }

            // 更新最後修改時間
            post.UpdatedAt = DateTime.UtcNow;

            // 儲存修改
            await _context.SaveChangesAsync();

            // 取得最新分類名稱
            var categoryName = await _context.Categories
                .Where(category => category.Id == post.CategoryId)
                .Select(category => category.Name)
                .FirstAsync();

            // 回傳更新後的文章 DTO
            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                IsPublished = post.IsPublished,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                PublishedAt = post.PublishedAt,
                CategoryId = post.CategoryId,
                CategoryName = categoryName
            };
        }

        // 刪除文章
        public async Task<bool> DeleteAsync(int id)
        {
            // 依照 ID 查詢文章
            var post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id == id);

            // 找不到文章
            if (post == null)
            {
                return false;
            }

            // 標記文章為刪除
            _context.Posts.Remove(post);

            // 儲存資料庫
            await _context.SaveChangesAsync();

            // 刪除成功
            return true;
        }
    }
}