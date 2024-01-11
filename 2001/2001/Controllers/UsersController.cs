using Microsoft.AspNetCore.Mvc;

namespace _2001.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
