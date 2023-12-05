using Microsoft.AspNetCore.Mvc;
using WebApplication2.DB;

namespace WebApplication2.Controllers
{
    public class LoginController : Controller
    {
        private readonly WebappDbContext _context;
        public LoginController(WebappDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Login()
        {

            return View();
        }
    }
}
