using Assignmen_PRN232_1.Services.IServices;
using Assignmen_PRN232__.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [Authorize(Roles = "Admin,Staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // GET api/newsarticles
        [HttpGet]
        public async Task<IActionResult> GetListPaging([FromQuery] NewsArticleSearchDto dto)
        {
            var result = await _newsArticleService.GetListPagingAsync(dto);
            return Ok(result);
        }

        // GET api/newsarticles/public
        [AllowAnonymous]
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicListPaging([FromQuery] NewsArticleSearchDto dto)
        {
            var result = await _newsArticleService.GetPublicListPagingAsync(dto);
            return Ok(result);
        }

        // GET api/newsarticles/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _newsArticleService.GetAllAsync();
            return Ok(result);
        }

        // GET api/newsarticles/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _newsArticleService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "News article not found" });
            return Ok(result);
        }

        // POST api/newsarticles/create-or-edit
        [HttpPost("create-or-edit")]
        public async Task<IActionResult> CreateOrEdit([FromBody] NewsArticleSaveDto dto)
        {
            var response = await _newsArticleService.CreateOrEditAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // POST api/newsarticles/{id}/duplicate
        [HttpPost("{id}/duplicate")]
        public async Task<IActionResult> Duplicate(string id)
        {
            var response = await _newsArticleService.DuplicateAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE api/newsarticles/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _newsArticleService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // POST api/newsarticles/{id}/tags/{tagId}
        [HttpPost("{id}/tags/{tagId:int}")]
        public async Task<IActionResult> AddTag(string id, int tagId)
        {
            var response = await _newsArticleService.AddTagAsync(id, tagId);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE api/newsarticles/{id}/tags/{tagId}
        [HttpDelete("{id}/tags/{tagId:int}")]
        public async Task<IActionResult> RemoveTag(string id, int tagId)
        {
            var response = await _newsArticleService.RemoveTagAsync(id, tagId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
