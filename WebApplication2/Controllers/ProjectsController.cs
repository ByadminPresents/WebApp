using Microsoft.AspNetCore.Mvc;
using WebApplication2.mocks;

namespace WebApplication2.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult ProjectsList()
        {
            var projects = new MockProject();
            return View(projects.AllProjects);
        }
    }
}
