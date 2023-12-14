using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
namespace WebApplication2
{
    public static class MailSender
    {
        public static async void SendInvites(string[] emails, string[] urls)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("sasha.maksimyuk", "eadv eqcc zkvo axyd");
                int count = 0;
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress("Приглашение на событие", "eventinviterutmn@yandex.ru"));
                msg.Subject = "Приглашение на событие";
                foreach (var email in emails)
                {
                    msg.To.Clear();
                    msg.To.Add(new MailboxAddress("", email));
                    msg.Body = new TextPart("html")
                    {
                        Text = $"<a href=\"{urls[count]}\">{urls[count]}</a>"
                    };
                    await client.SendAsync(msg);
                    count++;
                }
                await client.DisconnectAsync(true);
            }
        }
    }
}
