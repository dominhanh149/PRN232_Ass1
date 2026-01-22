using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Frontend.Services.IServices;

namespace Frontend.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7053/api/");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string Message, string? Token)> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new { accountEmail = email, accountPassword = password };
                var response = await _httpClient.PostAsJsonAsync("systemaccounts/login", loginDto);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login Response Status: {response.StatusCode}");
                Console.WriteLine($"Login Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    return (false, "Invalid email or password", null);
                }

                if (string.IsNullOrEmpty(responseContent))
                {
                    return (false, "Empty response from server", null);
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<SystemAccountDto>>();

                if (apiResponse == null || !apiResponse.Success)
                {
                    return (false, apiResponse?.Message ?? "Login failed", null);
                }

                // Save account info to cookies
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    httpContext.Response.Cookies.Append("UserName", apiResponse.Data?.AccountName ?? "", cookieOptions);
                    httpContext.Response.Cookies.Append("UserEmail", apiResponse.Data?.AccountEmail ?? "", cookieOptions);
                    httpContext.Response.Cookies.Append("UserRole", apiResponse.Data?.AccountRole.ToString() ?? "", cookieOptions);
                    httpContext.Response.Cookies.Append("IsAuthenticated", "true", cookieOptions);
                }

                return (true, apiResponse.Message ?? "Login successful", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Response.Cookies.Delete("UserName");
                    httpContext.Response.Cookies.Delete("UserEmail");
                    httpContext.Response.Cookies.Delete("UserRole");
                    httpContext.Response.Cookies.Delete("IsAuthenticated");
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LogoutAsync: {ex.Message}");
            }
        }
    }
}
