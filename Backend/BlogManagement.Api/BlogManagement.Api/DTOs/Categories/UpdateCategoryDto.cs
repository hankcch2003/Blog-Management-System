using System.ComponentModel.DataAnnotations;

namespace BlogManagement.Api.DTOs.Categories
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "分類名稱不可為空")]
        [StringLength(100, ErrorMessage = "分類名稱不可超過 100 個字元")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "分類描述不可超過 500 個字元")]
        public string? Description { get; set; }
    }
}