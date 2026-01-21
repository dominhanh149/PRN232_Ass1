using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232__.Dto
{
    public class CategoryDto
    {
        public short CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; } = null!;

        public bool? IsActive { get; set; }
    }

    public class CategorySaveDto
    {
        public short CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }

    public class CategorySearchDto : BaseSearchDto
    {
        // Kế thừa: Keyword, Status, PageIndex, PageSize
    }
}
