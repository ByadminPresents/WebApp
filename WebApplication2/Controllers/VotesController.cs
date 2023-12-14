using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.DB;

namespace WebApplication2.Controllers
{
    public class VotesController : Controller
    {
        private readonly WebappDbContext _context;
        public VotesController(WebappDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VotesView(string userId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            Viewer viewer = _context.Viewers.Include(e => e.UniqueKey).Include(e => e.VotingEvent).ThenInclude(e => e.Projects).Include(e => e.Votes).FirstOrDefault(e => e.UniqueKey.UniqueKeyValue == Crypto.GUIDShortener(userId));
            if (viewer != null)
            {
                var votes = new Dictionary<int, int>();
                foreach (var vote in viewer.Votes)
                {
                    votes.Add(vote.ProjectId, vote.Score);
                }
                ViewData["votes"] = votes;
                ViewData["userId"] = userId;
                return View("Vote", viewer.VotingEvent);
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult VotesApply(string userId, int teamId, int score)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            Viewer viewer = _context.Viewers.Include(e => e.UniqueKey).Include(e => e.VotingEvent).ThenInclude(e => e.Projects).Include(e => e.Votes).FirstOrDefault(e => e.UniqueKey.UniqueKeyValue == Crypto.GUIDShortener(userId));
            if (viewer != null && score <= 5 && score >= 1 && viewer.VotingEvent.Projects.FirstOrDefault(e => e.Id == teamId) != null)
            {
                var vote = viewer.Votes.FirstOrDefault(e => e.Project.Id == teamId);
                if (vote != null)
                {
                    vote.Score = score;
                    //_context.Update(vote.Score);
                }
                else
                {
                    _context.Votes.Add(new Vote { Score = score, ProjectId = teamId, ViewerId = (int)viewer.Id});
                }
                _context.SaveChanges();
                return RedirectToAction("VotesView", new { userId = userId });
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
