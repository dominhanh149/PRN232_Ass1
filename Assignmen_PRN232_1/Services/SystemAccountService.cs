using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232__.Enums;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.DTOs.Common;
using Assignmen_PRN232_1.Services.IServices;
using Mapster;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Assignmen_PRN232_1.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository, IConfiguration configuration)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<SystemAccountDto>> GetAllAsync()
        {
            var accounts = await _systemAccountRepository.GetAllAsync();

            var result = accounts.Adapt<IEnumerable<SystemAccountDto>>();

            return result;
        }

        public async Task<PagingResponse<SystemAccountDto>> GetListPagingAsync(SystemAccountSearchDto dto)
        {
            var pagedData = await _systemAccountRepository.GetListPagingAsync(dto);

            return new PagingResponse<SystemAccountDto>
            {
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize,
                TotalRecords = pagedData.TotalRecords,
                Items = pagedData.Items.Adapt<IEnumerable<SystemAccountDto>>()
            };
        }

        public async Task<SystemAccountDto?> GetByIdAsync(short id)
        {
            var account = await _systemAccountRepository.GetByIdAsync(id);
            if (account == null)
                return null;

            var dto = account.Adapt<SystemAccountDto>();
            dto.AccountRoleName = GetRoleName(account.AccountRole);

            return dto;
        }

        /// <summary>
        /// Router - Create hoặc Update dựa vào ID
        /// </summary>
        public async Task<ApiResponse<SystemAccountDto>> CreateOrEditAsync(SystemAccountSaveDto dto)
        {
            return dto.AccountId == 0
                ? await CreateAsync(dto)
                : await UpdateAsync(dto);
        }

        /// <summary>
        /// Login API - Kiểm tra email & password
        /// </summary>
        public async Task<ApiResponse<SystemAccountDto>> LoginAsync(SystemAccountLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AccountEmail) || string.IsNullOrWhiteSpace(dto.AccountPassword))
                return ApiResponse<SystemAccountDto>.Fail("Email and password are required");

            var defaultAdmin = GetAdminAccount();

            if (dto.AccountEmail == defaultAdmin.Email && dto.AccountPassword == defaultAdmin.Password)
            {
                return ApiResponse<SystemAccountDto>.Ok(new SystemAccountDto
                {
                    AccountId = 0,
                    AccountEmail = defaultAdmin.Email,
                    AccountName = defaultAdmin.Name,
                    AccountRole = defaultAdmin.Role,
                    AccountRoleName = "Admin"
                }, "Login successfully");
            }

            var account = await _systemAccountRepository.GetByEmailAsync(dto.AccountEmail);
            if (account == null)
                return ApiResponse<SystemAccountDto>.Fail("Invalid email or password");


            // So sánh password
            if (account.AccountPassword != dto.AccountPassword)
                return ApiResponse<SystemAccountDto>.Fail("Invalid email or password");

            var resultDto = account.Adapt<SystemAccountDto>();
            resultDto.AccountRoleName = GetRoleName(account.AccountRole);

            return ApiResponse<SystemAccountDto>.Ok(resultDto, "Login successfully");
        }

        #region Private

        private async Task<ApiResponse<SystemAccountDto>> CreateAsync(SystemAccountSaveDto dto)
        {
            // Kiểm tra email đã tồn tại chưa
            var exists = await _systemAccountRepository.ExistsByEmailAsync(dto.AccountEmail);
            if (exists)
                return ApiResponse<SystemAccountDto>.Fail("Email already exists");

            if (string.IsNullOrWhiteSpace(dto.AccountPassword))
                return ApiResponse<SystemAccountDto>.Fail("Password is required");

            // DTO -> Entity
            var entity = dto.Adapt<SystemAccount>();

            await _systemAccountRepository.AddAsync(entity);
            await _systemAccountRepository.SaveChangesAsync();

            var resultDto = entity.Adapt<SystemAccountDto>();
            resultDto.AccountRoleName = GetRoleName(entity.AccountRole);

            return ApiResponse<SystemAccountDto>.Ok(resultDto, "Created successfully");
        }

        private async Task<ApiResponse<SystemAccountDto>> UpdateAsync(SystemAccountSaveDto dto)
        {
            var existing = await _systemAccountRepository.GetByIdAsync(dto.AccountId);
            if (existing == null)
                return ApiResponse<SystemAccountDto>.Fail("Account not found");

            // Kiểm tra email nếu bị thay đổi
            if (!string.IsNullOrEmpty(dto.AccountEmail) && dto.AccountEmail != existing.AccountEmail)
            {
                var emailExists = await _systemAccountRepository.ExistsByEmailAsync(dto.AccountEmail);
                if (emailExists)
                    return ApiResponse<SystemAccountDto>.Fail("Email already exists");
            }

            // Update thông tin (không cập nhật password nếu không có)
            existing.AccountName = dto.AccountName ?? existing.AccountName;
            existing.AccountEmail = dto.AccountEmail ?? existing.AccountEmail;
            existing.AccountRole = dto.AccountRole ?? existing.AccountRole;

            // Chỉ update password nếu có giá trị mới
            if (!string.IsNullOrWhiteSpace(dto.AccountPassword))
            {
                existing.AccountPassword = dto.AccountPassword;
            }

            await _systemAccountRepository.UpdateAsync(existing);
            await _systemAccountRepository.SaveChangesAsync();

            var resultDto = existing.Adapt<SystemAccountDto>();
            resultDto.AccountRoleName = GetRoleName(existing.AccountRole);

            return ApiResponse<SystemAccountDto>.Ok(
                resultDto,
                "Updated successfully"
            );
        }

        private DefaultAdminConfig GetAdminAccount()
        {
            var email = _configuration["DefaultAdmin:AccountEmail"];
            var password = _configuration["DefaultAdmin:AccountPassword"];
            var role = _configuration["DefaultAdmin:AccountRole"];

            return new DefaultAdminConfig
            {
                Email = email,
                Password = password,
                Role = int.Parse(role),
            };
        }

        #endregion

        public async Task<ApiResponse<bool>> DeleteAsync(short id)
        {
            var account = await _systemAccountRepository.GetByIdAsync(id);
            if (account == null)
                return ApiResponse<bool>.Fail("Account not found");

            await _systemAccountRepository.DeleteAsync(account);
            await _systemAccountRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Deleted successfully");
        }

        /// <summary>
        /// Get role name từ role ID
        /// </summary>
        private string GetRoleName(int? roleId)
        {
            return roleId switch
            {
                (int)AccountRole.Admin => "Admin",
                (int)AccountRole.Staff => "Staff",
                (int)AccountRole.Lecturer => "Lecturer",
                _ => "Unknown"
            };
        }
    }
}
