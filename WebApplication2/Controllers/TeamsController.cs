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
            return View("TeamCreate", new Project() { VotingEventId = votingEventId });
        }

        [HttpPost]
        public async Task<IActionResult> TeamCreate(Project project)
        {
            for (int i = 0; i < project.Participants.Count; i++)
            {
                if (project.Participants.ToArray()[i].Name == null || project.Participants.ToArray()[i].Name == "")
                project.Participants.Remove(project.Participants.ToArray()[i]);
            }

            _context.Projects.Add(project);

            var uniqueKeys = new UniqueKey[project.Participants.Count];

            for (int i = 0; i < project.Participants.Count; i++)
            {
                uniqueKeys[i] = new UniqueKey() { UniqueKeyValue = Guid.NewGuid().ToString() };
                //uniqueKeys[i] = new UniqueKey() { UniqueKeyValue = "not_unique_key" };
            }

            foreach (UniqueKey x in uniqueKeys)
            {
                bool identityFlag = true;
                do
                {
                    identityFlag = true;
                    try
                    {
                        _context.UniqueKeys.Add(x);
                        _context.SaveChanges();
                    }
                    catch
                    {
                        identityFlag = false;
                        x.UniqueKeyValue = Guid.NewGuid().ToString();
                    }
                } while (!identityFlag);
            }

            int count = 0;
            foreach (var x in project.Participants)
            {
                x.UniqueKeyId = uniqueKeys[count].Id;
                _context.Update(x);
                count++;
            }

            _context.SaveChanges();
            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = project.VotingEventId });
        }
    }
}