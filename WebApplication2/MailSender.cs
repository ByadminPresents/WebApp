using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
namespace WebApplication2
{
    public static class MailSender
    {
        public static async void SendInvites(List<string> emails, List<string> urls)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync("eventinviterutmn", "mwrbhsrsyromizlk");
                int count = 0;
                foreach (var email in emails)
                {
                    var msg = new MimeMessage();
                    msg.Body = new TextPart("Plain")
                    {
                        Text = urls[count]
                    };
                    await client.SendAsync(msg);
                    count++;
                }
                await client.DisconnectAsync(true);
            }
        }
    }
}
