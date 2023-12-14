using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.DB;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Viewers).ThenInclude(e => e.Email).Include(e => e.Viewers).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
            if (votingEvent != null)
            {
                ViewData["votingEventId"] = votingEventId;
                return View("ViewersEdit", votingEvent.Viewers);
            }
            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
        }
        public IActionResult ViewersViewList(int votingEventId, List<Viewer> viewers)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["votingEventId"] = votingEventId;
            return View("ViewersEdit", viewers);
        }

        [HttpPost]
        public IActionResult UpdateViewers(int votingEventId, List<Viewer> viewers, string buttonParams = "")
        {
            var buttonValues = buttonParams.Split(',');
            foreach (var v in buttonValues)
            {
                if (v != "" && Int32.TryParse(v, out int id))
                {
                    var tempViewer = _context.Viewers.Include(e => e.Email).Include(e => e.UniqueKey).Include(e => e.Votes).FirstOrDefault(e => e.Id == id && e.VotingEventId == votingEventId);
                    _context.Votes.RemoveRange(tempViewer.Votes);
                    if (tempViewer.UniqueKey != null)
                    {
                        _context.UniqueKeys.Remove(tempViewer.UniqueKey);
                    }
                    _context.Viewers.Remove(tempViewer);
                }
            }
            _context.SaveChanges();
            foreach (var viewer in viewers)
            {
                if (viewer.Id != null)
                {
                    var tempViewer = _context.Viewers.Include(e => e.Email).FirstOrDefault(e => e.Id == viewer.Id && e.VotingEventId == votingEventId);
                    if (tempViewer == null)
                    {
                        return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
                    }
                    tempViewer.Name = viewer.Name;
                    tempViewer.Email.EmailValue = viewer.Email.EmailValue;
                }
                else
                {
                    viewer.VotingEventId = votingEventId;
                    _context.Viewers.Add(viewer);
                }
            }
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
            return ViewersViewList(votingEventId, viewers);
        }

        public IActionResult ViewersInvitesView(int votingEventId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Viewers).ThenInclude(e => e.Email).Include(e => e.Viewers).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
            ViewData["votingEventId"] = votingEventId;
            return View("ViewersInvite", votingEvent.Viewers);
        }

        public IActionResult ViewersSendInvites(int votingEventId, string buttonParams)
        {
            try
            {
                var buttonParameters = buttonParams.Split(':');
                if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
                {
                    return RedirectToAction("Index", "Login");
                }
                VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Viewers).ThenInclude(e => e.Email).Include(e => e.Viewers).ThenInclude(e => e.Votes).Include(e => e.Viewers).ThenInclude(e => e.UniqueKey).FirstOrDefault(e => e.Id == votingEventId);
                if (votingEvent != null)
                {
                    switch (buttonParameters[0])
                    {
                        case "sendNew":
                            {
                                CreateURLs(votingEvent.Viewers.Where(e => e.UniqueKey == null).ToList(), true);
                                break;
                            }
                        case "sendAll":
                            {
                                CreateURLs(votingEvent.Viewers.ToList(), true);
                                break;
                            }
                        case "sendSingle":
                            {
                                if (Int32.TryParse(buttonParameters[1], out int vId))
                                {
                                    CreateURLs(votingEvent.Viewers.Where(e => e.Id == vId).ToList(), true);
                                }
                                break;
                            }
                    }
                }
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
            return ViewersInvitesView(votingEventId);
        }

        private bool CreateURLs(List<Viewer> viewers, bool sendEmail)
        {
            if (viewers.Count == 0)
            {
                return false;
            }
            var uniqueKeys = new UniqueKey[viewers.Count];

            for (int i = 0; i < viewers.Count; i++)
            {
                uniqueKeys[i] = new UniqueKey() { UniqueKeyValue = Guid.NewGuid().ToString() };
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
            foreach (var viewer in viewers)
            {
                if (viewer.UniqueKey != null)
                {
                    _context.UniqueKeys.Remove(viewer.UniqueKey);
                }
                viewer.UniqueKeyId = uniqueKeys[count].Id;
                _context.Update(viewer);
                count++;
            }
            _context.SaveChanges();
            if (sendEmail)
            {
                string[] emails = new string[viewers.Count], urls = new string[viewers.Count];
                count = 0;
                foreach (var viewer in viewers)
                {
                    emails[count] = viewer.Email.EmailValue;
                    urls[count] = $"https://localhost:44343/Votes/VotesView?userId={Crypto.GUIDLengthifyer(viewer.UniqueKey.UniqueKeyValue)}";
                    count++;
                }
                MailSender.SendInvites(emails, urls);
            }
            return true;
        }

        public IActionResult ViewersSendSingleInvite(int votingEventId, int viewerId)
        {


            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
        }
    }
}
