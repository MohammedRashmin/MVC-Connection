using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("https://localhost:7275/Login/Login?returnUrl=https://localhost:7155/Home/Index");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var username = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var role = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    ViewBag.Username = username;
                    ViewBag.Role = role;

                    return View();
                }
            }
            catch (Exception ex)
            {
                // log error
            }

            return Redirect("https://localhost:7275/Login/Login?returnUrl=https://localhost:7155/Home/Index");
        }

    }
}

