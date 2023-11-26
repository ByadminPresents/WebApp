using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IAllVotes
    {
        IEnumerable<Viewer.Vote> AllVotes { get; }
    }
}
