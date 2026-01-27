using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232_1.Services.IServices
{
    public interface ISystemAccountService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<PagingDto<SystemAccountDto>>> GetAllAccountsAsync(SearchSystemAccountDto searchDto);
        Task<ApiResponse<SystemAccountDto>> GetAccountByIdAsync(short accountId);
        Task<ApiResponse<SystemAccountDto>> CreateAccountAsync(CreateSystemAccountDto createDto);
        Task<ApiResponse<SystemAccountDto>> UpdateAccountAsync(UpdateSystemAccountDto updateDto);
        Task<ApiResponse<bool>> DeleteAccountAsync(short accountId);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<bool> IsEmailExistsAsync(string email, short? excludeAccountId = null);
        Task<bool> CanDeleteAccountAsync(short accountId);
        Task<ApiResponse<SystemAccountDto>> GetAccountByEmailAsync(string email);
    }
}