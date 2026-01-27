using Assignmen_PRN232__.Models;
using Assignmen_PRN232_1.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Assignmen_PRN232__.Dto
{
    public class NewsArticleDto
    {
        public string NewsArticleID { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedByID { get; set; }
        public string? CreatedByName { get; set; }
        public short? UpdatedByID { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }

    // DTO for creating news article
    public class CreateNewsArticleDto
    {
        [Required(ErrorMessage = "News Article ID is required")]
        [StringLength(20, ErrorMessage = "News Article ID cannot exceed 20 characters")]
        public string NewsArticleID { get; set; } = null!;

        [StringLength(400, ErrorMessage = "News title cannot exceed 400 characters")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = null!;

        [StringLength(4000, ErrorMessage = "News content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [StringLength(400, ErrorMessage = "News source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryID { get; set; }

        public bool NewsStatus { get; set; } = true;

        public List<int> TagIDs { get; set; } = new List<int>();
    }

    // DTO for updating news article
    public class UpdateNewsArticleDto
    {
        [Required(ErrorMessage = "News Article ID is required")]
        [StringLength(20, ErrorMessage = "News Article ID cannot exceed 20 characters")]
        public string NewsArticleID { get; set; } = null!;

        [StringLength(400, ErrorMessage = "News title cannot exceed 400 characters")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = null!;

        [StringLength(4000, ErrorMessage = "News content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [StringLength(400, ErrorMessage = "News source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryID { get; set; }

        public bool NewsStatus { get; set; }

        public List<int> TagIDs { get; set; } = new List<int>();
    }

    // DTO for duplicating news article
    public class DuplicateNewsArticleDto
    {
        [Required(ErrorMessage = "Source News Article ID is required")]
        public string SourceNewsArticleID { get; set; } = null!;

        [Required(ErrorMessage = "New News Article ID is required")]
        [StringLength(20, ErrorMessage = "News Article ID cannot exceed 20 characters")]
        public string NewNewsArticleID { get; set; } = null!;
    }

    // DTO for news article search
    public class SearchNewsArticleDto : BaseSearchDto
    {
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public string? NewsContent { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public bool? NewsStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public short? CategoryID { get; set; }
        public short? CreatedByID { get; set; }
    }

    // DTO for related news
    public class RelatedNewsDto
    {
        public string NewsArticleID { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
    }

    // DTO for news history (for staff to view their created articles)
    public class NewsHistoryDto
    {
        public string NewsArticleID { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
    }
}
