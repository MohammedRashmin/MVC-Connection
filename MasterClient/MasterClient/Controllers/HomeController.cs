using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MasterClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Get the token from the cookie
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("https://localhost:7275/Login/Login?returnUrl=https://localhost:7155/Home/Index");
            }

            try
            {
                // Validate the token
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    // Validate the token's claims and extract user information
                    var username = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var role = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Set claims to the ViewBag or use them as needed
                    ViewBag.Username = username;
                    ViewBag.Role = role;

                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Token validation failed", ex);
            }

            // Redirect to login if token validation fails
            return Redirect("https://localhost:7275/Login/Login?returnUrl=https://localhost:7155/Home/Index");
        }
    }
}
