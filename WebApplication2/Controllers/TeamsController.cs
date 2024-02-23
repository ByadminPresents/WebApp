using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication2.DB;
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

        //[HttpGet]
        //public IActionResult TeamsList(int votingEventId)
        //{
        //    VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Projects).ThenInclude(e => e.Participants).Include(e => e.Projects).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
        //    if (votingEvent != null)
        //    {
        //        var totalVotes = new List<double>();
        //        foreach(var x in votingEvent.Projects)
        //        {
        //            totalVotes.Add(x.Votes.Count);
        //        }
        //        ViewBag.TotalVotes = totalVotes;
        //        return View(votingEvent);
        //    }
        //    return View(nameof(HomeController));
        //}


        [HttpGet]
        public IActionResult TeamCreateView(int votingEventId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["votingEventId"] = votingEventId;
            return View("TeamCreate", new Project() { VotingEventId = votingEventId });
        }

        [HttpGet]
        public IActionResult TeamEditView(int votingEventId, int projectId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            var project = _context.Projects.Include(e => e.Users).FirstOrDefault(e => e.Id == projectId && e.VotingEventId == votingEventId);
            if (project != null)
            {
                ViewData["votingEventId"] = votingEventId;
                return View("TeamEdit", project);
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> TeamCreate(Project project)
        {
            for (int i = 0; i < project.Users.Count; i++)
            {
                if (project.Users.ToArray()[i].Name == null || project.Users.ToArray()[i].Name == "")
                    project.Users.Remove(project.Users.ToArray()[i]);
            }

            _context.Projects.Add(project);
            int count = 0;
            foreach (var x in project.Users)
            {
                x.UniqueKey = Guid.NewGuid().ToString();
                x.Role = Convert.ToInt32("10", 2);
                count++;
            }

            _context.SaveChanges();
            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = project.VotingEventId });
        }
        [HttpPost]
        public async Task<IActionResult> TeamEdit(Project project, string buttonParams = "")
        {
            var buttonValues = buttonParams.Split(',');
            foreach (var v in buttonValues)
            {
                if (v != "" && Int32.TryParse(v, out int id))
                {
                    var tempParticipant = _context.Users.FirstOrDefault(e => e.Id == id && e.ProjectId == project.Id);
                    if (tempParticipant != null)
                    {
                        _context.Users.Remove(tempParticipant);
                    }
                }
            }
            var contextProject = _context.Projects.Include(e => e.Users).FirstOrDefault(e => e.Id == project.Id);
            if (contextProject != null)
            {
                contextProject.Title = project.Title;
                contextProject.Description = project.Description;
                var newParticipants = project.Users.Where(p => p.Id == null).ToList();
                var oldParticipants = project.Users.Where(p => p.Id != null).ToList();
                for (int i = 0; i < newParticipants.Count; i++)
                {
                    if (newParticipants[i].Name == null || newParticipants[i].Name == "")
                    {
                        newParticipants.Remove(newParticipants[i]);
                    }
                }

                int count = 0;
                foreach (var x in newParticipants)
                {
                    x.UniqueKey = Guid.NewGuid().ToString();
                    x.ProjectId = project.Id;
                    x.Role = Convert.ToInt32("10", 2);
                    _context.Users.Add(x);
                    count++;
                }

                foreach (var x in oldParticipants)
                {
                    var tempParticipant = _context.Users.FirstOrDefault(e => e.Id == x.Id && e.ProjectId == project.Id);
                    if (tempParticipant != null)
                    {
                        tempParticipant.Name = x.Name;
                        tempParticipant.Email = x.Email;
                        _context.Update(tempParticipant);
                    }
                }
                _context.Update(contextProject);

                _context.SaveChanges();
                return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = project.VotingEventId });
            }
            return RedirectToAction("Index", "Login");
        }
        public async Task<IActionResult> TeamDelete(int votingEventId, int projectId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            var project = _context.Projects.Include(e => e.Users).Include(e => e.Votes).FirstOrDefault(e => e.Id == projectId && e.VotingEventId == votingEventId);
            if (project != null)
            {
                _context.Users.RemoveRange(project.Users);
                _context.Votes.RemoveRange(project.Votes);
                _context.Projects.Remove(project);
                _context.SaveChanges();
                return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = project.VotingEventId });
            }
            return RedirectToAction("Index", "Login");
        }
    }
}