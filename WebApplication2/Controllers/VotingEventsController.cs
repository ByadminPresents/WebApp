using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Linq;
using WebApplication2.DB;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class VotingEventsController : Controller
    {
        private readonly WebappDbContext _context;
        public VotingEventsController(WebappDbContext context)
        {
            _context = context;
        }
        public IActionResult VotingEventsList()
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            var votingEvents = _context.VotingEvents.Include(e => e.Projects).Include(e => e.Organizer).ThenInclude(e => e.UniqueKey).Where(e => e.Organizer.UniqueKey.UniqueKeyValue == Crypto.GetUserIdFromSessionID(Request.Cookies["sessionId"]));
            if (votingEvents != null)
            {
                return View(votingEvents);
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult VotingEventCreate()
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VotingEventCreate(VotingEvent votingEvent)
        {
            var organizer = _context.Organizers.Include(e => e.UniqueKey).FirstOrDefault(e => e.UniqueKey.UniqueKeyValue == Crypto.GetUserIdFromSessionID(Request.Cookies["sessionId"]));
            votingEvent.OrganizerId = organizer.Id;
            _context.VotingEvents.Add(votingEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VotingEventsList));
            //return View(votingEvent);
        }

        [HttpGet]
        public IActionResult VotingEventEdit(int votingEventId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Organizer).ThenInclude(e => e.UniqueKey).Include(e => e.Projects).ThenInclude(e => e.Participants).Include(e => e.Projects).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId &&
            e.Organizer.UniqueKey.UniqueKeyValue == Crypto.GetUserIdFromSessionID(Request.Cookies["sessionId"]));
            if (votingEvent != null)
            {
                var totalVotes = new List<double>();
                foreach (var x in votingEvent.Projects)
                {
                    totalVotes.Add(x.Votes.Count);
                }
                ViewBag.TotalVotes = totalVotes;
                var projects = _context.Projects.Where(e => e.VotingEventId == votingEventId);
                var tempProjects = new List<KeyValuePair<int, double>>();
                int count = 1;
                foreach (var project in projects)
                {
                    double sum = 0;
                    foreach (var vote in project.Votes)
                    {
                        sum += vote.Score;
                    }
                    tempProjects.Add(new KeyValuePair<int, double>(count, sum / project.Votes.Count));
                    count++;
                }

                ViewData["projects"] = tempProjects.OrderByDescending(x => x.Value).ToList();
                //var projectValues = new double[projects.Count()];
                //var projectNames = new string[projects.Count()];
                //count = 0;
                //foreach (var project in projects)
                //{
                //    projectNames[count] = project.Title;
                //    double sum = 0;
                //    foreach (var vote in project.Votes)
                //    {
                //        sum += vote.Score;
                //    }
                //    projectValues[count] = sum / project.Votes.Count;
                //    count++;
                //}
                //ViewData["projectValues"] = projectValues;
                //ViewData["projectNames"] = projectNames;
                return View("VotingEventEdit", votingEvent);
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
