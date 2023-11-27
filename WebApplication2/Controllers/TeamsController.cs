using Microsoft.AspNetCore.Mvc;
using WebApplication2.mocks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class TeamsController : Controller
    {
        public IActionResult TeamsList()
        {
            var teams = new MockTeam();
            return View();
        }
    }
}
