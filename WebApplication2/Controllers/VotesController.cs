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
            //if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            User viewer = _context.Users.Include(e => e.VotingEvent).ThenInclude(e => e.Projects).Include(e => e.Votes).FirstOrDefault(e => e.UniqueKey == Crypto.GUIDShortener(userId));
            if (viewer != null)
            {
                var votes = new List<KeyValuePair<int, KeyValuePair<int, int>>>();
                foreach (var vote in viewer.Votes)
                {
                    votes.Add(new KeyValuePair<int, KeyValuePair<int, int>>(vote.ProjectId, new KeyValuePair<int, int>(vote.Score, vote.Criteria)));
                }
                ViewData["votes"] = votes;
                ViewData["userId"] = userId;
                return View("Vote", viewer.VotingEvent);
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult VotesApply(string userId, int teamId, int score, int criteriaId)
        {
            //if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            User viewer = _context.Users.Include(e => e.VotingEvent).ThenInclude(e => e.Projects).Include(e => e.Votes).FirstOrDefault(e => e.UniqueKey == Crypto.GUIDShortener(userId));
            if (viewer != null && score <= 5 && score >= 1 && viewer.VotingEvent.Projects.FirstOrDefault(e => e.Id == teamId) != null)
            {
                var vote = viewer.Votes.FirstOrDefault(e => e.Project.Id == teamId && e.Criteria == criteriaId);
                if (vote != null)
                {
                    vote.Score = score;
                    vote.Criteria = criteriaId;
                    //_context.Update(vote.Score);
                }
                else
                {
                    _context.Votes.Add(new Vote { Score = score, ProjectId = teamId, ViewerId = (int)viewer.Id, Criteria = criteriaId});
                }
                _context.SaveChanges();
                return RedirectToAction("VotesView", new { userId = userId });
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult VotesCriteriasView(int votingEventId)
        {
            try
            {
                if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
                {
                    return RedirectToAction("Index", "Login");
                }
                VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Users).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
                if (votingEvent != null)
                {
                    ViewData["votingEventId"] = votingEvent.Id;
                    List<KeyValuePair<string, double>> criterias = new List<KeyValuePair<string, double>>();
                    if (votingEvent.Criterias == "")
                    {
                        votingEvent.Criterias = "Оценка:1";
                        _context.Update(votingEvent);
                        _context.SaveChanges();
                    }
                    
                    foreach (string x in votingEvent.Criterias.Split(';'))
                    {
                        string[] tempStr = x.Split(':');
                        criterias.Add(new KeyValuePair<string, double>(tempStr[0], Convert.ToDouble(tempStr[1])));
                    }
                    ViewData["votingCriterias"] = criterias;
                    return View("VoteCriteriasEdit");
                }
            }
            catch
            {

            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult UpdateCriterias(int votingEventId, List<KeyValuePair<string, double>> criterias)
        {
            try
            {
                if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
                {
                    return RedirectToAction("Index", "Login");
                }
                VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Users).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
                if (votingEvent != null)
                {
                    string stringCriterias = "";
                    int count = 0;
                    foreach (var x in criterias)
                    {
                        if (count > 0)
                        {
                            stringCriterias += ";";
                        }
                        stringCriterias += x.Key + ":" + x.Value;
                        count++;
                    }
                    votingEvent.Criterias = stringCriterias;
                    _context.Update(votingEvent);
                    _context.SaveChanges();
                    return RedirectToAction("VotesCriteriasView", "Votes", new { votingEventId = votingEvent.Id });
                }
            }
            catch
            {

            }
            return RedirectToAction("Index", "Login");
        }
    }
}
