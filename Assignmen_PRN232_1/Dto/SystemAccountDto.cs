using Assignmen_PRN232_1.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Assignmen_PRN232__.Dto
{
    public class SystemAccountDto
    {
        public short AccountId { get; set; }

        public string? AccountName { get; set; }

        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }

        public string? AccountRoleName { get; set; }
    }

    public class SystemAccountSaveDto
    {

    }

    public class SystemAccountLoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? AccountPassword { get; set; }
    }

    public class SystemAccountSearchDto : BaseSearchDto
    {
        public int? AccountRole { get; set; }
    }

    public class DefaultAdminConfig
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Role { get; set; }
    }

}
