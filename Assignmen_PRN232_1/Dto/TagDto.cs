using Assignmen_PRN232_1.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Assignmen_PRN232__.Dto
{
    public class TagDto
    {
        public int TagId { get; set; }

        public string? TagName { get; set; }

        public string? Note { get; set; }
    }

    public class TagSaveDto
    {
        public int TagId { get; set; }

        [Required(ErrorMessage = "Tag name is required")]
        [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
        public string? TagName { get; set; }

        [StringLength(200, ErrorMessage = "Note cannot exceed 200 characters")]
        public string? Note { get; set; }
    }

    public class TagSearchDto : BaseSearchDto
    {
        
    }
}
