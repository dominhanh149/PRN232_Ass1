using Assignmen_PRN232__.Models;
using Assignmen_PRN232_1.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Assignmen_PRN232__.Dto
{
    public class CategoryDto
    {
        public short CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryDesciption { get; set; } = null!;
        public short? ParentCategoryID { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public int ArticleCount { get; set; } // Number of articles in this category
    }

    // DTO for creating category
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Category description is required")]
        [StringLength(250, ErrorMessage = "Category description cannot exceed 250 characters")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryID { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // DTO for updating category
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Category ID is required")]
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Category description is required")]
        [StringLength(250, ErrorMessage = "Category description cannot exceed 250 characters")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryID { get; set; }

        public bool IsActive { get; set; }
    }

    // DTO for category search
    public class SearchCategoryDto : BaseSearchDto
    {
        public string? CategoryName { get; set; }
        public string? CategoryDesciption { get; set; }
        public bool? IsActive { get; set; }
    }

    // DTO for toggling category status
    public class ToggleCategoryStatusDto
    {
        [Required(ErrorMessage = "Category ID is required")]
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "IsActive status is required")]
        public bool IsActive { get; set; }
    }
}
