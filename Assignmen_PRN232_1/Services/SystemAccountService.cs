//using Assignmen_PRN232__.Dto;
//using Assignmen_PRN232__.Dto.Common;
//using Assignmen_PRN232__.Enums;
//using Assignmen_PRN232__.Models;
//using Assignmen_PRN232_1.DTOs.Common;
//using Assignmen_PRN232_1.Services.IServices;
//using Microsoft.EntityFrameworkCore;

//namespace Assignmen_PRN232_1.Services
//{
//    public class SystemAccountService : ISystemAccountService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IConfiguration _configuration;
//        private readonly string _adminEmail;
//        private readonly string _adminPassword;

//        public SystemAccountService(IUnitOfWork unitOfWork, IConfiguration configuration)
//        {
//            _unitOfWork = unitOfWork;
//            _configuration = configuration;

//            // Get admin credentials from appsettings.json
//            _adminEmail = _configuration["AdminAccount:Email"] ?? "admin@FUNewsManagementSystem.org";
//            _adminPassword = _configuration["AdminAccount:Password"] ?? "@@abc123@@";
//        }

//        // ===== AUTHENTICATION =====
//        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
//        {
//            try
//            {
//                // Check if it's admin account from appsettings.json
//                if (loginDto.AccountEmail == _adminEmail && loginDto.AccountPassword == _adminPassword)
//                {
//                    var adminResponse = new LoginResponseDto
//                    {
//                        AccountID = 0,
//                        AccountName = "Administrator",
//                        AccountEmail = _adminEmail,
//                        AccountRole = (int)AccountRole.Admin,
//                        RoleName = "Admin",
//                        Token = GenerateToken(0, _adminEmail, "Admin")
//                    };
//                    return ApiResponse<LoginResponseDto>.SuccessResponse(adminResponse, "Login successful");
//                }

//                // Check regular account in database
//                var accounts = await _unitOfWork.SystemAccountRepository
//                    .GetByConditionAsync(a => a.AccountEmail == loginDto.AccountEmail);

//                var account = accounts.FirstOrDefault();

//                if (account == null || account.AccountPassword != loginDto.AccountPassword)
//                {
//                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password");
//                }

//                var roleName = account.AccountRole == (int)AccountRole.Staff ? "Staff" : "Lecturer";

//                var response = new LoginResponseDto
//                {
//                    AccountID = account.AccountID,
//                    AccountName = account.AccountName ?? "",
//                    AccountEmail = account.AccountEmail ?? "",
//                    AccountRole = account.AccountRole ?? 0,
//                    RoleName = roleName,
//                    Token = GenerateToken(account.AccountID, account.AccountEmail ?? "", roleName)
//                };

//                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<LoginResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
//            }
//        }

//        // ===== GET ALL ACCOUNTS WITH SEARCH & PAGINATION =====
//        public async Task<ApiResponse<PagingDto<SystemAccountDto>>> GetAllAccountsAsync(SearchSystemAccountDto searchDto)
//        {
//            try
//            {
//                var query = _unitOfWork.SystemAccountRepository.GetAll();

//                // Apply filters
//                if (!string.IsNullOrWhiteSpace(searchDto.AccountName))
//                {
//                    query = query.Where(a => a.AccountName != null && a.AccountName.Contains(searchDto.AccountName));
//                }

//                if (!string.IsNullOrWhiteSpace(searchDto.AccountEmail))
//                {
//                    query = query.Where(a => a.AccountEmail != null && a.AccountEmail.Contains(searchDto.AccountEmail));
//                }

//                if (searchDto.AccountRole.HasValue)
//                {
//                    query = query.Where(a => a.AccountRole == searchDto.AccountRole.Value);
//                }

//                // Get total count before pagination
//                var totalCount = await query.CountAsync();

//                // Apply sorting
//                if (!string.IsNullOrWhiteSpace(searchDto.SortBy))
//                {
//                    query = searchDto.SortBy.ToLower() switch
//                    {
//                        "accountname" => searchDto.SortDescending
//                            ? query.OrderByDescending(a => a.AccountName)
//                            : query.OrderBy(a => a.AccountName),
//                        "accountemail" => searchDto.SortDescending
//                            ? query.OrderByDescending(a => a.AccountEmail)
//                            : query.OrderBy(a => a.AccountEmail),
//                        "accountrole" => searchDto.SortDescending
//                            ? query.OrderByDescending(a => a.AccountRole)
//                            : query.OrderBy(a => a.AccountRole),
//                        _ => query.OrderBy(a => a.AccountID)
//                    };
//                }
//                else
//                {
//                    query = query.OrderBy(a => a.AccountID);
//                }

//                // Apply pagination
//                var accounts = await query
//                    .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
//                    .Take(searchDto.PageSize)
//                    .ToListAsync();

//                // Map to DTOs
//                var accountDtos = accounts.Select(a => new SystemAccountDto
//                {
//                    AccountID = a.AccountID,
//                    AccountName = a.AccountName,
//                    AccountEmail = a.AccountEmail,
//                    AccountRole = a.AccountRole,
//                    RoleName = a.AccountRole == (int)AccountRole.Staff ? "Staff" : "Lecturer"
//                }).ToList();

//                var pagingResult = new PagingDto<SystemAccountDto>
//                {
//                    Items = accountDtos,
//                    TotalCount = totalCount,
//                    PageNumber = searchDto.PageNumber,
//                    PageSize = searchDto.PageSize
//                };

//                return ApiResponse<PagingDto<SystemAccountDto>>.SuccessResponse(pagingResult);
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<PagingDto<SystemAccountDto>>.ErrorResponse($"Error retrieving accounts: {ex.Message}");
//            }
//        }

//        // ===== GET ACCOUNT BY ID =====
//        public async Task<ApiResponse<SystemAccountDto>> GetAccountByIdAsync(short accountId)
//        {
//            try
//            {
//                var account = await _unitOfWork.SystemAccountRepository.GetByIdAsync(accountId);

//                if (account == null)
//                {
//                    return ApiResponse<SystemAccountDto>.ErrorResponse("Account not found");
//                }

//                var accountDto = new SystemAccountDto
//                {
//                    AccountID = account.AccountID,
//                    AccountName = account.AccountName,
//                    AccountEmail = account.AccountEmail,
//                    AccountRole = account.AccountRole,
//                    RoleName = account.AccountRole == (int)AccountRole.Staff ? "Staff" : "Lecturer"
//                };

//                return ApiResponse<SystemAccountDto>.SuccessResponse(accountDto);
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<SystemAccountDto>.ErrorResponse($"Error retrieving account: {ex.Message}");
//            }
//        }

//        // ===== CREATE ACCOUNT =====
//        public async Task<ApiResponse<SystemAccountDto>> CreateAccountAsync(CreateSystemAccountDto createDto)
//        {
//            try
//            {
//                // Check if email already exists
//                if (await IsEmailExistsAsync(createDto.AccountEmail))
//                {
//                    return ApiResponse<SystemAccountDto>.ErrorResponse("Email already exists");
//                }

//                // Find next available AccountID (since it's not IDENTITY)
//                var maxId = await _unitOfWork.SystemAccountRepository.GetAll()
//                    .MaxAsync(a => (short?)a.AccountID) ?? 0;

//                var newAccount = new SystemAccount
//                {
//                    AccountID = (short)(maxId + 1),
//                    AccountName = createDto.AccountName,
//                    AccountEmail = createDto.AccountEmail,
//                    AccountRole = createDto.AccountRole,
//                    AccountPassword = createDto.AccountPassword
//                };

//                await _unitOfWork.SystemAccountRepository.AddAsync(newAccount);
//                await _unitOfWork.SaveChangesAsync();

//                var accountDto = new SystemAccountDto
//                {
//                    AccountID = newAccount.AccountID,
//                    AccountName = newAccount.AccountName,
//                    AccountEmail = newAccount.AccountEmail,
//                    AccountRole = newAccount.AccountRole,
//                    RoleName = newAccount.AccountRole == (int)AccountRole.Staff ? "Staff" : "Lecturer"
//                };

//                return ApiResponse<SystemAccountDto>.SuccessResponse(accountDto, "Account created successfully");
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<SystemAccountDto>.ErrorResponse($"Error creating account: {ex.Message}");
//            }
//        }

//        // ===== UPDATE ACCOUNT =====
//        public async Task<ApiResponse<SystemAccountDto>> UpdateAccountAsync(UpdateSystemAccountDto updateDto)
//        {
//            try
//            {
//                var account = await _unitOfWork.SystemAccountRepository.GetByIdAsync(updateDto.AccountID);

//                if (account == null)
//                {
//                    return ApiResponse<SystemAccountDto>.ErrorResponse("Account not found");
//                }

//                // Check if email already exists for another account
//                if (await IsEmailExistsAsync(updateDto.AccountEmail, updateDto.AccountID))
//                {
//                    return ApiResponse<SystemAccountDto>.ErrorResponse("Email already exists");
//                }

//                account.AccountName = updateDto.AccountName;
//                account.AccountEmail = updateDto.AccountEmail;
//                account.AccountRole = updateDto.AccountRole;

//                _unitOfWork.SystemAccountRepository.Update(account);
//                await _unitOfWork.SaveChangesAsync();

//                var accountDto = new SystemAccountDto
//                {
//                    AccountID = account.AccountID,
//                    AccountName = account.AccountName,
//                    AccountEmail = account.AccountEmail,
//                    AccountRole = account.AccountRole,
//                    RoleName = account.AccountRole == (int)AccountRole.Staff ? "Staff" : "Lecturer"
//                };

//                return ApiResponse<SystemAccountDto>.SuccessResponse(accountDto, "Account updated successfully");
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<SystemAccountDto>.ErrorResponse($"Error updating account: {ex.Message}");
//            }
//        }

//        // ===== DELETE ACCOUNT =====
//        public async Task<ApiResponse<bool>> DeleteAccountAsync(short accountId)
//        {
//            try
//            {
//                var account = await _unitOfWork.SystemAccountRepository.GetByIdAsync(accountId);

//                if (account == null)
//                {
//                    return ApiResponse<bool>.ErrorResponse("Account not found");
//                }

//                // Check if account has created any articles
//                if (!await CanDeleteAccountAsync(accountId))
//                {
//                    return ApiResponse<bool>.ErrorResponse(
//                        "Cannot delete account because it has created news articles. Please reassign or delete the articles first.");
//                }

//                _unitOfWork.SystemAccountRepository.Delete(account);
//                await _unitOfWork.SaveChangesAsync();

//                return ApiResponse<bool>.SuccessResponse(true, "Account deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<bool>.ErrorResponse($"Error deleting account: {ex.Message}");
//            }
//        }

//        // ===== CHANGE PASSWORD =====
//        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
//        {
//            try
//            {
//                var account = await _unitOfWork.SystemAccountRepository.GetByIdAsync(changePasswordDto.AccountID);

//                if (account == null)
//                {
//                    return ApiResponse<bool>.ErrorResponse("Account not found");
//                }

//                // Verify current password
//                if (account.AccountPassword != changePasswordDto.CurrentPassword)
//                {
//                    return ApiResponse<bool>.ErrorResponse("Current password is incorrect");
//                }

//                account.AccountPassword = changePasswordDto.NewPassword;

//                _unitOfWork.SystemAccountRepository.Update(account);
//                await _unitOfWork.SaveChangesAsync();

//                return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<bool>.ErrorResponse($"Error changing password: {ex.Message}");
//            }
//        }

//        // ===== VALIDATION HELPERS =====
//        public async Task<bool> IsEmailExistsAsync(string email, short? excludeAccountId = null)
//        {
//            var query = _unitOfWork.SystemAccountRepository.GetAll()
//                .Where(a => a.AccountEmail == email);

//            if (excludeAccountId.HasValue)
//            {
//                query = query.Where(a => a.AccountID != excludeAccountId.Value);
//            }

//            return await query.AnyAsync();
//        }

//        public async Task<bool> CanDeleteAccountAsync(short accountId)
//        {
//            var hasArticles = await _unitOfWork.NewsArticleRepository.GetAll()
//                .AnyAsync(na => na.CreatedByID == accountId);

//            return !hasArticles;
//        }

//        public async Task<ApiResponse<SystemAccountDto>> GetAccountByEmailAsync(string email)
//        {
//            try
//            {
//                var accounts = await _unitOfWork.SystemAccountRepository
//                    .GetByConditionAsync(a => a.AccountEmail == email);

//                var account = accounts.FirstOrDefault();

//                if (account == null)
//                {
//                    return ApiResponse<SystemAccountDto>.ErrorResponse("Account not found");
//                }

//                var accountDto = new SystemAccountDto
//                {
//                    AccountID = account.AccountID,
//                    AccountName = account.AccountName,
//                    AccountEmail = account.AccountEmail,
//                    AccountRole = account.AccountRole,
//                    RoleName = account.AccountRole == (int)AccountRole.Staff ? "Staff" : "Lecturer"
//                };

//                return ApiResponse<SystemAccountDto>.SuccessResponse(accountDto);
//            }
//            catch (Exception ex)
//            {
//                return ApiResponse<SystemAccountDto>.ErrorResponse($"Error retrieving account: {ex.Message}");
//            }
//        }

//        // ===== HELPER METHODS =====
//        private string GenerateToken(short accountId, string email, string role)
//        {
//            // Simple token generation for now
//            // In production, use JWT tokens
//            return Convert.ToBase64String(
//                System.Text.Encoding.UTF8.GetBytes($"{accountId}:{email}:{role}:{DateTime.UtcNow.Ticks}")
//            );
//        }
//    }
//}