using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class ViewersController : Controller
    {
        public IActionResult AddViewersView()
        {

            return View();
        }

        [HttpPost]
        public IActionResult AddViewers()
        {

            return View();
        }
    }
}
