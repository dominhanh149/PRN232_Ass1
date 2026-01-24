using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [Authorize(Roles = "Staff, Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tagService.GetAllAsync();
            return Ok(result);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetListPaging([FromQuery] TagSearchDto dto)
        {
            var result = await _tagService.GetListPagingAsync(dto);
            return Ok(ApiResponse<object>.Ok(result, "Get list successfully"));
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _tagService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<object>.Fail("Tag not found", StatusCodes.Status404NotFound));
            return Ok(ApiResponse<TagDto>.Ok(result, "Get tag successfully"));
        }

        
        [HttpPost("create-or-edit")]
        public async Task<IActionResult> CreateOrEdit([FromBody] TagSaveDto dto)
        {
            var response = await _tagService.CreateOrEditAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _tagService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
