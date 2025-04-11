using System.Diagnostics;
using MasterAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MasterAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Ensure the user is authenticated with JWT token
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // Ensure the user is authenticated with JWT token
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Optionally, you can create a Logout method to handle JWT token removal from the client-side storage
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the AuthToken cookie and redirect to Login page
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login", "Login");
        }
    }
}
