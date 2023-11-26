using WebApplication2.Interfaces;
using WebApplication2.Models;

namespace WebApplication2.mocks
{
    public class MockParticipant : IAllParticipants
    {
        public IEnumerable<Participant> AllParticipants
        {
            get
            {
                return new List<Participant>
                {
                    new Participant { Id = 1, Name = "Иванов Иван Иванович", Email = "tempemail@mail.ru"}
                };
            }
        }
    }
}
