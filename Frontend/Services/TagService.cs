using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.DTOs.Common;
using Frontend.Services.IServices;

namespace Frontend.Services
{
    public class TagService : ITagService
    {
        private readonly HttpClient _httpClient;

        public TagService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7053/api/");
        }

        // GET: Lấy danh sách phân trang
        public async Task<PagingResponse<TagDto>> GetListPagingAsync(TagSearchDto searchDto)
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

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

                var response = await _httpClient.GetAsync($"Tags{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return new PagingResponse<TagDto>();
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<PagingResponse<TagDto>>>();

                return apiResponse?.Data ?? new PagingResponse<TagDto>();
            }
            catch (Exception ex)
            {
                // Log error
                return new PagingResponse<TagDto>();
            }
        }

        // GET: Lấy tất cả (không phân trang) - nếu cần
        public async Task<List<TagDto>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Tags/all");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<TagDto>();
                }

                var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>();
                return tags ?? new List<TagDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return new List<TagDto>();
            }
        }

        // GET: Lấy theo ID
        public async Task<TagDto?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Tags/{id}");

                if (!response.IsSuccessStatusCode)
                { 
                    return null;
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TagDto>>();

                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        // POST: Tạo mới hoặc cập nhật
        public async Task<(bool Success, string Message, TagDto? Data)> CreateOrEditAsync(TagSaveDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Tags/create-or-edit", dto);

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<TagDto>>();

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

        // DELETE: Xóa tag
        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Tags/{id}");

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