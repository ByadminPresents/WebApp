using WebApplication2.Interfaces;
using WebApplication2.Models;

namespace WebApplication2.mocks
{
    public class MockVote : IAllVotes
    {
        public IEnumerable<Viewer.Vote> AllVotes
        {
            get
            {
                return new List<Viewer.Vote>
                {
                    new Viewer.Vote { VoterId = 1, Criteria = 1, Score = 1, TeamId = 1 }
                };
            }
        }
    }
}
