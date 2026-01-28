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
        private readonly ISystemAccountService _service;

        public SystemAccountsController(ISystemAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetListPaging([FromQuery] SystemAccountSearchDto dto)
        {
            var result = await _service.GetListPagingAsync(dto);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(short id)
        {
            var acc = await _service.GetByIdAsync(id);
            if (acc == null) return NotFound();
            return Ok(acc);
        }

        [HttpPost("create-or-edit")]
        public async Task<IActionResult> CreateOrEdit([FromBody] SystemAccountSaveDto dto)
        {
            var response = await _service.CreateOrEditAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // ===== LOGIN =====
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SystemAccountLoginDto dto)
        {
            var response = await _service.LoginAsync(dto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);

            var acc = response.Data;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, acc.AccountId.ToString()),
                new Claim(ClaimTypes.Name, acc.AccountEmail!),
                new Claim(ClaimTypes.Role,
                    acc.AccountRole == 0 ? "Admin" :
                    acc.AccountRole == 1 ? "Staff" : "Lecturer")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Ok(response);
        }
    }
}
