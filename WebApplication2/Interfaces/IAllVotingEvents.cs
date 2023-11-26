using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IAllVotingEvents
    {
        IEnumerable<VotingEvent> AllVotingEvents { get; }
    }
}
