using System.ComponentModel.DataAnnotations;

namespace BlogManagement.Api.DTOs.Posts
{
    public class UpdatePostDto
    {
        [Required(ErrorMessage = "文章標題不可為空")]
        [StringLength(200, ErrorMessage = "文章標題不可超過 200 個字元")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "文章內容不可為空")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "文章摘要不可超過 500 個字元")]
        public string? Summary { get; set; }

        public bool IsPublished { get; set; } = false;

        [Range(1, int.MaxValue, ErrorMessage = "請選擇有效的文章分類")]
        public int CategoryId { get; set; }
    }
}