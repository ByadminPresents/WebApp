using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IAllParticipants
    {
        IEnumerable<Participant> AllParticipants { get; }
    }
}
