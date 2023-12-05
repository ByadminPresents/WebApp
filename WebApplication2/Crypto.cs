using Microsoft.EntityFrameworkCore;
using WebApplication2.DB;

namespace WebApplication2
{
    public static class Crypto
    {

        public static bool CheckSessionID(string sessionId)
        {
            var context = new WebappDbContext();

            if (context.Organizers.Include(e => e.UniqueKey).FirstOrDefault(e => e.UniqueKey.UniqueKeyValue == sessionId) == null)
            {
                return false;
            }
            return true;
        }

        public static string GetSessionID(string login, string password) 
        {
            var context = new WebappDbContext();

            var item = context.Organizers.Include(e => e.Email).Include(e => e.UniqueKey).FirstOrDefault(e => e.Email.EmailValue == login && e.Password == password);

            if (item == null)
            {
                return null;
            }

            return item.UniqueKey.UniqueKeyValue;
        }
    }
}
