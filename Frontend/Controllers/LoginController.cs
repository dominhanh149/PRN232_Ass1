using Frontend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        // GET: Login/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Email and password are required";
                return RedirectToAction(nameof(Index));
            }

            var result = await _loginService.LoginAsync(email, password);

            if (result.Success)
            {
                // Ensure session is committed before redirect
                HttpContext.Session.CommitAsync();
                
                // Debug: Check session values
                Console.WriteLine($"Session UserName: {HttpContext.Session.GetString("UserName")}");
                Console.WriteLine($"Session UserEmail: {HttpContext.Session.GetString("UserEmail")}");
                Console.WriteLine($"Session UserRole: {HttpContext.Session.GetString("UserRole")}");
                
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Login/Logout
        public async Task<IActionResult> Logout()
        {
            await _loginService.LogoutAsync();
            TempData["SuccessMessage"] = "Logged out successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
