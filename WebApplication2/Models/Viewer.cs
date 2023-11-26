namespace WebApplication2.Models
{
    public class Viewer
    {
        public struct Vote
        {
            public int VoterId { get; set; }
            public int Score { get; set; }
            public int Criteria { get; set; }
            public int TeamId { get; set; }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
