using Assignmen_PRN232_1.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Assignmen_PRN232__.Dto
{
    public class ReportRequestDto
    {
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public string? GroupBy { get; set; } // "Category", "Author", "Status"
    }

    // DTO for report by category
    public class ReportByCategoryDto
    {
        public short CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
    }

    // DTO for report by author
    public class ReportByAuthorDto
    {
        public short CreatedByID { get; set; }
        public string CreatedByName { get; set; } = null!;
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
    }

    // DTO for report by status
    public class ReportByStatusDto
    {
        public bool NewsStatus { get; set; }
        public string StatusName { get; set; } = null!;
        public int TotalArticles { get; set; }
    }

    // DTO for overall report
    public class OverallReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
        public List<ReportByCategoryDto>? ByCategory { get; set; }
        public List<ReportByAuthorDto>? ByAuthor { get; set; }
        public List<ReportByStatusDto>? ByStatus { get; set; }
    }

    // DTO for article list in report
    public class ReportArticleDto
    {
        public string NewsArticleID { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public bool? NewsStatus { get; set; }
    }
}