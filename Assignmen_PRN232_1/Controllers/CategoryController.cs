using Assignmen_PRN232_1.Services.IServices;
using Assignmen_PRN232__.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET api/categories
        [HttpGet]
        public async Task<IActionResult> GetListPaging([FromQuery] CategorySearchDto dto)
        {
            var result = await _categoryService.GetListPagingAsync(dto);
            return Ok(result);
        }

        // GET api/categories/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Category not found" });
            return Ok(result);
        }

        // POST api/categories/create-or-edit
        [HttpPost("create-or-edit")]
        public async Task<IActionResult> CreateOrEdit([FromBody] CategorySaveDto dto)
        {
            var response = await _categoryService.CreateOrEditAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _categoryService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
