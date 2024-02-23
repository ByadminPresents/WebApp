using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Linq;
using WebApplication2.DB;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class IntDoubleData
    {
        public int x = 0;
        public double y = 0;
        public IntDoubleData(int x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
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
            var votingEvents = _context.VotingEvents.Include(e => e.Projects).Include(e => e.Organizer).Where(e => e.Organizer.UniqueKey == Crypto.GetUserIdFromSessionID(Request.Cookies["sessionId"]));
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
            var organizer = _context.Users.FirstOrDefault(e => e.UniqueKey == Crypto.GetUserIdFromSessionID(Request.Cookies["sessionId"]));
            votingEvent.OrganizerId = (int)organizer.Id;
            votingEvent.Criterias = "Оценка:1";
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
            VotingEvent votingEvent = _context.VotingEvents.Include(e => e.Organizer).Include(e => e.Projects).ThenInclude(e => e.Users).Include(e => e.Projects).ThenInclude(e => e.Votes).FirstOrDefault(e => e.Id == votingEventId &&
            e.Organizer.UniqueKey == Crypto.GetUserIdFromSessionID(Request.Cookies["sessionId"]));
            if (votingEvent != null)
            {
                var criteriasWeights = new Dictionary<int, double>();
                var criteriaNames = new Dictionary<int, string>();
                int index = 0;
                foreach (string x in votingEvent.Criterias.Split(';'))
                {
                    string[] tempStr = x.Split(':');
                    criteriaNames.Add(index, tempStr[0]);
                    criteriasWeights.Add(index, Convert.ToDouble(tempStr[1]));
                    index++;
                }

                var votesDict = new Dictionary<int, Dictionary<int, List<KeyValuePair<int, double>>>>();
                var projectVotesValues = new Dictionary<int, double[]>();

                var totalVotes = new Dictionary<int, int>();

                var votes = _context.Votes.Include(e => e.Project).Where(e => e.Project.VotingEventId == votingEventId).ToList();
                foreach (var vote in votes)
                {
                    if (!votesDict.ContainsKey(vote.ProjectId))
                    {
                        votesDict.Add(vote.ProjectId, new Dictionary<int, List<KeyValuePair<int, double>>>());
                    }
                    if (!votesDict[vote.ProjectId].ContainsKey(vote.ViewerId))
                    {
                        votesDict[vote.ProjectId].Add(vote.ViewerId, new List<KeyValuePair<int, double>>());
                    }
                    votesDict[vote.ProjectId][vote.ViewerId].Add(new KeyValuePair<int, double>(vote.Criteria, vote.Score));
                }

                var viewersMaxScoreDict = new Dictionary<int, IntDoubleData>();
                var projectsCriteriasAverageValues = new Dictionary<int, IntDoubleData[]>();
                var viewersNames = new Dictionary<int, string>();
                foreach (var i in votesDict)
                {
                    projectVotesValues.Add(i.Key, new double[] { 0, 0 });
                    projectsCriteriasAverageValues.Add(i.Key, new IntDoubleData[criteriasWeights.Count]);
                    for (int dataI = 0; dataI < criteriasWeights.Count; dataI++)
                    {
                        projectsCriteriasAverageValues[i.Key][dataI] = new IntDoubleData(0, 0);
                    }
                    totalVotes.Add(i.Key, 0);
                    foreach (var n in i.Value)
                    {
                        if (!viewersNames.ContainsKey(n.Key))
                        {
                            viewersNames.Add(n.Key, _context.Users.FirstOrDefault(x => x.Id == n.Key).Name);
                            viewersMaxScoreDict.Add(n.Key, new IntDoubleData(0, -1));
                        }
                        projectVotesValues[i.Key][1] += 1;
                        totalVotes[i.Key] += 1;
                        double weightsSum = 0, scoreSum = 0;
                        foreach (var m in n.Value)
                        {
                            if (criteriasWeights.ContainsKey(m.Key))
                            {
                                weightsSum += criteriasWeights[m.Key];
                                scoreSum += m.Value * criteriasWeights[m.Key];
                                projectsCriteriasAverageValues[i.Key][m.Key].x++;
                                projectsCriteriasAverageValues[i.Key][m.Key].y += m.Value;
                            }
                        }
                        double tempResult = scoreSum / weightsSum;
                        projectVotesValues[i.Key][0] += tempResult;
                        if (viewersMaxScoreDict[n.Key].y < tempResult)
                        {
                            viewersMaxScoreDict[n.Key].x = i.Key;
                            viewersMaxScoreDict[n.Key].y = tempResult;
                        }
                        else if (viewersMaxScoreDict[n.Key].y == tempResult)
                        {
                            viewersMaxScoreDict[n.Key].x = i.Key;
                            viewersMaxScoreDict[n.Key].y = -1;
                        }
                    }
                }

                ViewBag.ViewersNames = viewersNames;
                ViewBag.CriteriaNames = criteriaNames;
                ViewBag.ProjectsCriteriasAverageValues = projectsCriteriasAverageValues;
                ViewBag.ViewersMaxScoreDict = viewersMaxScoreDict;
                ViewBag.TotalVotes = totalVotes;

                var tempProjectsScores = new List<KeyValuePair<int, double>>();
                int count = 1;
                foreach (var project in votingEvent.Projects)
                {
                    if (projectVotesValues.ContainsKey(project.Id))
                    {
                        tempProjectsScores.Add(new KeyValuePair<int, double>(count, projectVotesValues[project.Id][0] / projectVotesValues[project.Id][1]));
                    }
                    else
                    {
                        tempProjectsScores.Add(new KeyValuePair<int, double>(count, 0));
                    }
                    count++;
                }

                ViewData["projects"] = tempProjectsScores.OrderByDescending(x => x.Value).ToList();
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
