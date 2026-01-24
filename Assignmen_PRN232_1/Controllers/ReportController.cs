using Assignmen_PRN232__.Dto;
using Assignmen_PRN232_1.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        
        [HttpGet("news-articles")]
        public async Task<IActionResult> GetNewsArticleReport([FromQuery] ReportSearchDto searchDto)
        {
            var report = await _reportService.GetNewsArticleReportAsync(searchDto);
            return Ok(report);
        }
    }
}
