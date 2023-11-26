using WebApplication2.Models;
namespace WebApplication2.Interfaces
{
    public interface IAllProjects
    {
        IEnumerable<Project> AllProjects { get; }
    }
}
