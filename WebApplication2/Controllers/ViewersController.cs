using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.DB;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;
using System.Text;

namespace WebApplication2.Controllers
{
    public class ViewersController : Controller
    {
        private readonly WebappDbContext _context;
        public ViewersController(WebappDbContext context)
        {
            _context = context;
        }
        public IActionResult ViewersView(int votingEventId, List<User> newUsers = null)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Users).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
            if (votingEvent != null)
            {
                ViewData["votingEventId"] = votingEventId;
                List<User> users = votingEvent.Users.ToList();
                if (newUsers != null)
                {
                    users.AddRange(newUsers);
                }
                return View("ViewersEdit", users);
            }
            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
        }
        public IActionResult ViewersViewList(int votingEventId, List<User> viewers)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["votingEventId"] = votingEventId;
            return View("ViewersEdit", viewers);
        }

        [HttpPost]
        public IActionResult UpdateViewers(int votingEventId, List<User> viewers, string buttonParams = "")
        {
            var buttonValues = buttonParams.Split(',');
            foreach (var v in buttonValues)
            {
                if (v != "" && Int32.TryParse(v, out int id))
                {
                    var tempViewer = _context.Users.Include(e => e.Votes).FirstOrDefault(e => e.Id == id && e.VotingEventId == votingEventId);
                    _context.Votes.RemoveRange(tempViewer.Votes);
                    _context.Users.Remove(tempViewer);
                }
            }
            _context.SaveChanges();
            foreach (var viewer in viewers)
            {
                if (viewer.Id != null)
                {
                    var tempViewer = _context.Users.FirstOrDefault(e => e.Id == viewer.Id && e.VotingEventId == votingEventId);
                    if (tempViewer == null)
                    {
                        return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
                    }
                    tempViewer.Name = viewer.Name;
                    tempViewer.Email = viewer.Email;
                }
                else
                {
                    viewer.VotingEventId = votingEventId;
                    viewer.Role = Convert.ToInt32("100", 2);
                    _context.Users.Add(viewer);
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
            return RedirectToAction("ViewersView", new { votingEventId = votingEventId });
        }

        public IActionResult ViewersInvitesView(int votingEventId)
        {
            if (Request.Cookies["sessionId"] == null || !Crypto.CheckSessionID(Request.Cookies["sessionId"]?.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Users).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
            ViewData["votingEventId"] = votingEventId;
            return View("ViewersInvite", votingEvent.Users);
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
                VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Users).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId);
                if (votingEvent != null)
                {
                    switch (buttonParameters[0])
                    {
                        case "sendNew":
                            {
                                CreateURLs(votingEvent.Users.Where(e => e.UniqueKey == null).ToList(), true);
                                break;
                            }
                        case "sendAll":
                            {
                                CreateURLs(votingEvent.Users.ToList(), true);
                                break;
                            }
                        case "sendSingle":
                            {
                                if (Int32.TryParse(buttonParameters[1], out int vId))
                                {
                                    CreateURLs(votingEvent.Users.Where(e => e.Id == vId).ToList(), true);
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

        private bool CreateURLs(List<User> viewers, bool sendEmail)
        {
            if (viewers.Count == 0)
            {
                return false;
            }

            int count = 0;
            foreach (var viewer in viewers)
            {
                viewer.UniqueKey = Guid.NewGuid().ToString();
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
                    emails[count] = viewer.Email;
                    urls[count] = $"https://192.168.0.108:80/Votes/VotesView?userId={Crypto.GUIDLengthifyer(viewer.UniqueKey)}";
                    count++;
                }
                MailSender.SendInvites(emails, urls);
            }
            return true;
        }

        public IActionResult ViewersUpload(int votingEventId)
        {
            ViewData["votingEventId"] = votingEventId;
            return View("ViewersUpload");
        }

        [HttpPost]
        public IActionResult ViewersUploadPost(int votingEventId, IFormFile uploadFile)
        {
            try
            {
                if (uploadFile != null && uploadFile.Length > 0)
                {
                    using (var streamReader = new StreamReader(uploadFile.OpenReadStream(), Encoding.GetEncoding(1251)))
                    {
                        List<User> users = new List<User>();
                        bool skipFirstLineFlag = true;
                        while (!streamReader.EndOfStream)
                        {
                            string line = streamReader.ReadLine();
                            if (skipFirstLineFlag)
                            {
                                skipFirstLineFlag = false;
                                continue;
                            }
                            users.Add(new User() { Name = line.Split(';')[0], Email = line.Split(';')[1], Role = Convert.ToInt32("100", 2) });
                        }
                        return ViewersView(votingEventId, users);
                    }
                }
                else
                {
                    return BadRequest("Файл не выбран или пуст.");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult ViewersSendSingleInvite(int votingEventId, int viewerId)
        {


            return RedirectToAction("VotingEventEdit", "VotingEvents", new { votingEventId = votingEventId });
        }
    }
}
