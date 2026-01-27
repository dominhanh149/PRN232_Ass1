using Assignmen_PRN232_1.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Assignmen_PRN232__.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string AccountEmail { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string AccountPassword { get; set; } = null!;
    }

    // ===== DTO for Login Response =====
    public class LoginResponseDto
    {
        public short AccountID { get; set; }
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
        public int AccountRole { get; set; }
        public string RoleName { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    // ===== DTO for reading account (without password) =====
    public class SystemAccountDto
    {
        public short AccountID { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
        public string? RoleName { get; set; }
    }

    // ===== DTO for creating account =====
    public class CreateSystemAccountDto
    {
        [Required(ErrorMessage = "Account name is required")]
        [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string AccountEmail { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer)")]
        public int AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(70, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 70 characters")]
        public string AccountPassword { get; set; } = null!;
    }

    // ===== DTO for updating account =====
    public class UpdateSystemAccountDto
    {
        [Required(ErrorMessage = "Account ID is required")]
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string AccountEmail { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer)")]
        public int AccountRole { get; set; }
    }

    // ===== DTO for changing password =====
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Account ID is required")]
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(70, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 70 characters")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class SearchSystemAccountDto : BaseSearchDto
    {
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
    }

}
