using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebApplication2.DB;

namespace WebApplication2
{
    public static class Crypto
    {
        private static readonly char[] chars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        public static bool CheckSessionID(string sessionId)
        {
            if (sessionId == null || sessionId == "" || sessionId.Length <= 42)
            {
                return false;
            }
            var decSessionId = DecryptData(sessionId);
            if (!ValidateCheckSum(decSessionId))
            {
                return false;
            }
            if (((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() - DecryptIDToTimestamp(decSessionId) >= 60 * 60 * 24)
            {
                return false;
            }
            var context = new WebappDbContext();
            if (context.Organizers.Include(e => e.UniqueKey).FirstOrDefault(e => e.UniqueKey.UniqueKeyValue == DecryptIDToUniqueKey(decSessionId)) == null)
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

            return EncryptID(item.UniqueKey.UniqueKeyValue);
        }
        public static string EncryptID(string sessionId)
        {
            if (sessionId.Length != 36)
            {
                return null;
            }
            string encSessionId = "";
            string[] parts = sessionId.Split('-');
            string timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString();
            string tempStr = "";
            int checkSum = 0;
            for (int i = 0; i < timestamp.Length; i++)
            {
                tempStr += chars[(Convert.ToInt32(timestamp[i]) + Array.IndexOf(chars, parts[2][i % 4])) % 16];
            }
            timestamp = tempStr;
            tempStr = "";
            for (int i = 0; i < parts[4].Length; i++)
            {
                if (timestamp.Length <= i)
                {
                    tempStr += parts[4].Substring(i);
                    break;
                }
                tempStr += $"{parts[4][i]}{timestamp[i]}";
            }
            parts[4] = tempStr;
            for (int i = 0; i < parts.Length; i++)
            {
                for (int n = 0; n < parts[i].Length; n++)
                {
                    checkSum += Array.IndexOf(chars, parts[i][n]);
                }
                if (i != 1)
                {
                    encSessionId += parts[i];
                }
            }
            tempStr = "";
            encSessionId += checkSum;
            for (int i = 0; i < encSessionId.Length; i++)
            {
                tempStr += chars[(Array.IndexOf(chars, encSessionId[i]) + Array.IndexOf(chars, parts[1][i % 4])) % 16];
            }
            encSessionId = "";
            int count = 0;
            for (int i = 0; i < tempStr.Length; i++)
            {
                encSessionId += tempStr[i];
                if (i == 3 || i == 9 || i == 13 || i == 22)
                {
                    encSessionId += parts[1][count];
                    count++;
                }
            }
            return encSessionId;
        }
        private static bool ValidateCheckSum(string decSessionId)
        {
            var parts = decSessionId.Split('-');
            int checkSum = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                for (int n = 0; n < parts[i].Length; n++)
                {
                    if (i < 4 || (i == 4 && n <= 21))
                    {
                        checkSum += Array.IndexOf(chars, parts[i][n]);
                    }
                }
            }
            return checkSum == Convert.ToInt32(parts[4].Substring(22));
        }
        private static long DecryptIDToTimestamp(string decSessionId)
        {
            var parts = decSessionId.Split('-');
            string timestamp = "";
            int count = 0;
            for (int i = 1; i < parts[4].Length; i += 2)
            {
                if (count == 10)
                {
                    break;
                }
                timestamp += chars[(Array.IndexOf(chars, parts[4][i]) + 16 - Array.IndexOf(chars, parts[2][count % 4])) % 16];
                count++;
            }
            return Convert.ToInt64(timestamp);
        }
        private static string DecryptIDToUniqueKey(string decSessionId)
        {
            var parts = decSessionId.Split('-');
            string uniqueKey = $"{parts[0]}-{parts[1]}-{parts[2]}-{parts[3]}-";
            int count = 0;
            for (int i = 0; i < parts[4].Length && count != 11; i += 2)
            {
                uniqueKey += parts[4][i];
                count++;
            }
            uniqueKey += parts[4][21];
            return uniqueKey;
        }

        private static string DecryptData(string encData)
        {
            string data = "";
            string tempStr = "";
            string part = $"{encData[4]}{encData[11]}{encData[16]}{encData[26]}";
            for (int i = 0; i < encData.Length; i++)
            {
                if (i == 4 || i == 11 || i == 16 || i == 26)
                {
                    continue;
                }
                tempStr += encData[i];
            }
            for (int i = 0; i < tempStr.Length; i++)
            {
                data += chars[(Array.IndexOf(chars, tempStr[i]) + 16 - Array.IndexOf(chars, part[i % 4])) % 16];
                if (i == 7)
                {
                    data += $"-{part}-";
                }
                else if (i == 11 || i == 15)
                {
                    data += '-';
                }
            }
            return data;
        }
    }
}
