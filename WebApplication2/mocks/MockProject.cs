using WebApplication2.Interfaces;
using WebApplication2.Models;

namespace WebApplication2.mocks
{
    public class MockProject : IAllProjects
    {
        private readonly IAllParticipants _allParticipants = new MockParticipant();
        public IEnumerable<Project> AllProjects
        {
            get
            {
                return new List<Project>
                {
                    new Project() { Id = 1, Name = "Название проекта", Participants = _allParticipants.AllParticipants.ToList()}
                };
            }
        }
    }
}
