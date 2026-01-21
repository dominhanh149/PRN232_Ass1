using Assignmen_PRN232_1.Services.IServices;
using Assignmen_PRN232__.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountsController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        // GET api/systemaccounts
        [HttpGet]
        public async Task<IActionResult> GetListPaging([FromQuery] SystemAccountSearchDto dto)
        {
            var result = await _systemAccountService.GetListPagingAsync(dto);
            return Ok(result);
        }

        // GET api/systemaccounts/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _systemAccountService.GetAllAsync();
            return Ok(result);
        }

        // GET api/systemaccounts/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(short id)
        {
            var result = await _systemAccountService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Account not found" });
            return Ok(result);
        }

        // POST api/systemaccounts/create-or-edit
        [HttpPost("create-or-edit")]
        public async Task<IActionResult> CreateOrEdit([FromBody] SystemAccountSaveDto dto)
        {
            var response = await _systemAccountService.CreateOrEditAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE api/systemaccounts/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _systemAccountService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // POST api/systemaccounts/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SystemAccountLoginDto dto)
        {
            var response = await _systemAccountService.LoginAsync(dto);
            return StatusCode(response.StatusCode, response);
        }
    }
}
