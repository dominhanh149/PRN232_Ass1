using Frontend.Models;
using Frontend.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITagService _tagService;

        public HomeController(ILogger<HomeController> logger, ITagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "PublicNews");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
