using Microsoft.AspNetCore.Mvc;
using WebApplication2.mocks;

namespace WebApplication2.Controllers
{
    public class VotingEventsController : Controller
    {
        public ViewResult VotingEventsList()
        {
            var votingEvents = new MockVotingEvent();
            return View(votingEvents.AllVotingEvents);
        }
    }
}
