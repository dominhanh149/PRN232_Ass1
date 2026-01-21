using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232__.Dto
{
    public class NewsArticleDto
    {
        public string NewsArticleId { get; set; }

        public string NewsArticleName { get; set; }

        public string? NewsTitle { get; set; }

        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

    public class NewsArticleSaveDto
    {
        public string NewsArticleId { get; set; }

        public string? NewsTitle { get; set; }

        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

    public class NewsArticleSearchDto : BaseSearchDto
    {
        public short? CategoryId { get; set; }
    }
}
