using Microsoft.AspNetCore.Mvc;
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
        public ViewResult VotingEventsList()
        {
            var votingEvents = _context.VotingEvents;
            return View(votingEvents);
        }

        public IActionResult VotingEventCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VotingEventCreate(VotingEvent votingEvent)
        {
            if (ModelState.IsValid)
            {
                //throw new Exception();
                _context.Add(votingEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(votingEvent);
        }
    }
}
