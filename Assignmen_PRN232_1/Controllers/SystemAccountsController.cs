using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Enums;
using Assignmen_PRN232_1.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountsController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetListPaging([FromQuery] SystemAccountSearchDto dto)
        {
            var result = await _systemAccountService.GetListPagingAsync(dto);
            return Ok(result);
        }

        
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _systemAccountService.GetAllAsync();
            return Ok(result);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(short id)
        {
            var result = await _systemAccountService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Account not found" });
            return Ok(result);
        }

        
        [HttpPost("create-or-edit")]
        public async Task<IActionResult> CreateOrEdit([FromBody] SystemAccountSaveDto dto)
        {
            var response = await _systemAccountService.CreateOrEditAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _systemAccountService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SystemAccountLoginDto dto)
        {
            var response = await _systemAccountService.LoginAsync(dto);

            if (!response.Success)
                return StatusCode(response.StatusCode, response);

            var account = response.Data;

            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.AccountEmail),
                new Claim(ClaimTypes.Role, ((AccountRole)account.AccountRole.GetValueOrDefault()).ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return StatusCode(response.StatusCode, response);
        }
    }
}
