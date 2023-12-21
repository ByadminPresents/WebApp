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
            var project = _context.Projects.Include(e => e.Participants).ThenInclude(e => e.Email).FirstOrDefault(e => e.Id == projectId && e.VotingEventId == votingEventId);
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
        [HttpPost]
        public async Task<IActionResult> TeamEdit(Project project, string buttonParams = "")
        {
            var buttonValues = buttonParams.Split(',');
            foreach (var v in buttonValues)
            {
                if (v != "" && Int32.TryParse(v, out int id))
                {
                    var tempParticipant = _context.Participants.Include(e => e.UniqueKey).Include(e => e.Email).FirstOrDefault(e => e.Id == id && e.ProjectId == project.Id);
                    if (tempParticipant != null)
                    {
                        _context.Emails.Remove(tempParticipant.Email);
                        _context.UniqueKeys.Remove(tempParticipant.UniqueKey);
                        _context.Participants.Remove(tempParticipant);
                    }
                }
            }
            var contextProject = _context.Projects.Include(e => e.Participants).ThenInclude(e => e.Email).FirstOrDefault(e => e.Id == project.Id);
            if (contextProject != null)
            {
                contextProject.Title = project.Title;
                contextProject.Description = project.Description;
                var newParticipants = project.Participants.Where(p => p.Id == null).ToList();
                var oldParticipants = project.Participants.Where(p => p.Id != null).ToList();
                for (int i = 0; i < newParticipants.Count; i++)
                {
                    if (newParticipants[i].Name == null || newParticipants[i].Name == "")
                    {
                        newParticipants.Remove(newParticipants[i]);
                    }
                }

                var uniqueKeys = new UniqueKey[newParticipants.Count];

                for (int i = 0; i < newParticipants.Count; i++)
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
                foreach (var x in newParticipants)
                {
                    x.UniqueKeyId = uniqueKeys[count].Id;
                    x.ProjectId = project.Id;
                    _context.Participants.Add(x);
                    count++;
                }

                foreach (var x in oldParticipants)
                {
                    var tempParticipant = _context.Participants.Include(e => e.Email).FirstOrDefault(e => e.Id == x.Id && e.ProjectId == project.Id);
                    if (tempParticipant != null)
                    {
                        tempParticipant.Name = x.Name;
                        tempParticipant.Email.EmailValue = x.Email?.EmailValue;
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
            var project = _context.Projects.Include(e => e.Participants).ThenInclude(e => e.Email).Include(e => e.Participants).ThenInclude(e => e.UniqueKey).Include(e => e.Votes).FirstOrDefault(e => e.Id == projectId && e.VotingEventId == votingEventId);
            if (project != null)
            {
                foreach (var x in project.Participants)
                {
                    _context.UniqueKeys.Remove(x.UniqueKey);
                    _context.Emails.Remove(x.Email);
                }
                _context.Participants.RemoveRange(project.Participants);
                _context.Votes.RemoveRange(project.Votes);
                _context.Projects.Remove(project);
                _context.SaveChanges();
                return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = project.VotingEventId });
            }
            return RedirectToAction("Index", "Login");
        }
    }
}