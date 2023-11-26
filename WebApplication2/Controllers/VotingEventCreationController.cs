using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class VotingEventCreationController : Controller
    {
        public IActionResult VotingEventCreate()
        {
            return View();
        }
    }
}
