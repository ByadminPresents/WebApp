using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
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
            var votingEvents = _context.VotingEvents.Include(e => e.Projects).Where(e => e.OrganizerId == 1);
            return View(votingEvents);
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
            votingEvent.OrganizerId = 1;
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
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Projects).ThenInclude(e => e.Participants).Include(e => e.Projects).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
            if (votingEvent != null)
            {
                var totalVotes = new List<double>();
                foreach (var x in votingEvent.Projects)
                {
                    totalVotes.Add(x.Votes.Count);
                }
                ViewBag.TotalVotes = totalVotes;
                return View("VotingEventEdit", votingEvent);
            }
            return View(nameof(HomeController));
        }
    }
}
