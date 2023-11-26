using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IAllViewers
    {
        IEnumerable<Viewer> AllViewers { get; }
    }
}
