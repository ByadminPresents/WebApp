namespace WebApplication2.Models
{
    public class VotingEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set;}
        public List<Project> Projects { get; set;} //TODO
        public List<Viewer> Viewers { get; set; } //TODO
    }
}
