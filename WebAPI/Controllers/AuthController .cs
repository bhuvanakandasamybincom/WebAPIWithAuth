using BoardCasterWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardCasterWebAPI.Controllers
{
    public class AuthController : Controller
    {
        [HttpPost("login")]
        public IActionResult Login()
        {
            JWTTokenService jWTTokenService = new JWTTokenService();
            var token = jWTTokenService.GenerateJwtToken();
            return Ok(new { token });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
