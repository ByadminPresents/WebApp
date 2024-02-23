using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication2.DB;

namespace WebApplication2.Controllers
{
    public class LoginController : Controller
    {
        private readonly WebappDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginController(WebappDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View("Login");
        }

        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var sessionId = Crypto.GetSessionID(login, password);
            if (sessionId == null)
            {
                ViewData["Login"] = login;
                return View("Login");
            }
            var options = new CookieOptions();
            options.Secure = true;
            options.Expires = DateTime.Now.AddHours(24);
            if (!Request.Cookies.ContainsKey("sessionId"))
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Append("sessionId", sessionId, options);
            }
            else
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("sessionId");
                _httpContextAccessor.HttpContext.Response.Cookies.Append("sessionId", sessionId, options);
            }
            return RedirectToAction("VotingEventsList", "VotingEvents");
        }

        [HttpPost]
        public IActionResult RegisterPost(string name, string login, string email, string password)
        {
            _context.Users.Add(new Models.User() { Name = name, Login = login, Email = email, Password = Crypto.GetPasswordHash(password), Role = Convert.ToInt32("1", 2), UniqueKey = Guid.NewGuid().ToString() });
            _context.SaveChanges();
            ViewData["Login"] = login;
            return View("Login");
        }
    }
}
