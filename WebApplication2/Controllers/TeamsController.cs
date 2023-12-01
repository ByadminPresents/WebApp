using Microsoft.AspNetCore.Mvc;
using WebApplication2.DB;
using WebApplication2.mocks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class TeamsController : Controller
    {
        private readonly WebappDbContext _context;
        public TeamsController(WebappDbContext context)
        {
            _context = context;
        }

        public IActionResult TeamsList(int votingEventId)
        {
            return View(_context.VotingEvents.Find(votingEventId));
        }

        [HttpGet]
        public IActionResult TeamCreateView(int votingEventId)
        {
            return View("TeamCreate", new Project() { VotingEventId = votingEventId });
        }

        [HttpPost]
        public async Task<IActionResult> TeamCreate(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
            var uniqueKeys = new string[project.Participants.Count];
            for (int i = 0; i < project.Participants.Count; i++)
            {
                uniqueKeys[i] = Guid.NewGuid().ToString();
            }
            foreach (var x in _context.Participants)
            {
                for (int i = 0; i < uniqueKeys.Length; i++)
                {
                    while (x.UniqueKey == uniqueKeys[i])
                    {
                        uniqueKeys[i] = Guid.NewGuid().ToString();
                    }
                }
            }
            int count = 0;
            foreach (var x in project.Participants)
            {
                x.UniqueKey = uniqueKeys[count];
                x.ProjectId = project.Id;
                _context.Participants.Add(x);
                count++;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeamsList));
        }
    }
}