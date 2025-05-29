using Microsoft.AspNetCore.Mvc;

namespace BoardCasterWebAPI.Controllers
{
    public class ContactSessionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
