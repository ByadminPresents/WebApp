using WebApplication2.Interfaces;
using WebApplication2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication2.mocks
{
    public class MockVotingEvent : IAllVotingEvents
    {
        private readonly IAllProjects _allTeams = new MockProject();
        private readonly IAllViewers _allViewers = new MockViewer();
        public IEnumerable<VotingEvent> AllVotingEvents
        {
            get
            {
                return new List<VotingEvent>
                {
                    new VotingEvent {Id = 1, Name = "Конференция 1", Description = "Описание для конференции 1", Date = DateTime.Now, Projects = _allTeams.AllProjects.ToList(), Viewers = _allViewers.AllViewers.ToList()}
                };
            }
        }
    }
}
