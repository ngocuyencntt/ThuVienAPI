using Microsoft.AspNetCore.Mvc;

namespace Library_web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
