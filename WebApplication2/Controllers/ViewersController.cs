using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.DB;

namespace WebApplication2.Controllers
{
    public class ViewersController : Controller
    {
        private readonly WebappDbContext _context;
        public ViewersController(WebappDbContext context)
        {
            _context = context;
        }
        public IActionResult ViewersView(int votingEventId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Viewers).ThenInclude(e => e.Email).FirstOrDefault(e => e.Id == votingEventId);
            if (votingEvent != null)
            {
                ViewData["votingEventId"] = votingEventId;
                return View("Viewers", votingEvent.Viewers);
            }
            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
        }

        [HttpPost]
        public IActionResult UpdateViewers(int votingEventId, List<Viewer> viewers)
        { 
            foreach (var viewer in viewers)
            {
                if (viewer.Id != null)
                {
                    var tempViewer = _context.Viewers.Include(e => e.Email).FirstOrDefault(e => e.Id == viewer.Id);
                    tempViewer.Name = viewer.Name;
                    tempViewer.Email.EmailValue = viewer.Email.EmailValue;
                }
                else
                {
                    viewer.VotingEventId = votingEventId;
                    _context.Viewers.Add(viewer);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
        }

        [HttpPost]
        public IActionResult AddViewers()
        {

            return View();
        }
    }
}
