using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.DTOs.Common;
using Frontend.Services.IServices;

namespace Frontend.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly HttpClient _httpClient;

        public SystemAccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7053/api/");
        }

        // GET: Lấy danh sách phân trang
        public async Task<PagingResponse<SystemAccountDto>> GetListPagingAsync(SystemAccountSearchDto searchDto)
        {
            try
            {
                // Build query string
                var queryParams = new List<string>();
                if (searchDto.PageIndex > 0)
                    queryParams.Add($"PageIndex={searchDto.PageIndex}");
                if (searchDto.PageSize > 0)
                    queryParams.Add($"PageSize={searchDto.PageSize}");
                if (!string.IsNullOrEmpty(searchDto.Keyword))
                    queryParams.Add($"Keyword={Uri.EscapeDataString(searchDto.Keyword)}");
                if (searchDto.AccountRole.HasValue)
                    queryParams.Add($"AccountRole={searchDto.AccountRole}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

                var response = await _httpClient.GetAsync($"SystemAccounts{queryString}");

                Console.WriteLine($"GetListPaging Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new PagingResponse<SystemAccountDto>();
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<PagingResponse<SystemAccountDto>>();

                return apiResponse ?? new PagingResponse<SystemAccountDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetListPagingAsync: {ex.Message}");
                return new PagingResponse<SystemAccountDto>();
            }
        }

        // GET: Lấy tất cả (không phân trang)
        public async Task<List<SystemAccountDto>> GetAllAsync()
        {
            try
            {
                var searchDto = new SystemAccountSearchDto
                {
                    PageIndex = 1,
                    PageSize = 1000
                };

                var result = await GetListPagingAsync(searchDto);
                return result.Items?.ToList() ?? new List<SystemAccountDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return new List<SystemAccountDto>();
            }
        }

        // GET: Lấy theo ID
        public async Task<SystemAccountDto?> GetByIdAsync(short id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SystemAccounts/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<SystemAccountDto>();

                return apiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        // POST: Tạo mới hoặc cập nhật
        public async Task<(bool Success, string Message, SystemAccountDto? Data)> CreateOrEditAsync(SystemAccountSaveDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("SystemAccounts/create-or-edit", dto);

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<SystemAccountDto>>();

                if (apiResponse == null)
                {
                    return (false, "Failed to process response", null);
                }

                return (apiResponse.Success, apiResponse.Message ?? "", apiResponse.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateOrEditAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        // DELETE: Xóa system account
        public async Task<(bool Success, string Message)> DeleteAsync(short id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SystemAccounts/{id}");

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<bool>>();

                if (apiResponse == null)
                {
                    return (false, "Failed to process response");
                }

                return (apiResponse.Success, apiResponse.Message ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}
