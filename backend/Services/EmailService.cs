using MailKit.Net.Smtp;
using MimeKit;

namespace StitchArtisan.Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            
            // Generate HTML wrapped body
            var htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #fcf9f8; color: #333; margin: 0; padding: 0; }}
                    .container {{ max-w: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; border: 1px solid #e0d5ce; margin-top: 20px; margin-bottom: 20px; }}
                    .header {{ text-align: center; padding-bottom: 20px; border-bottom: 1px solid #e0d5ce; }}
                    .header h1 {{ color: #b36a3a; margin: 0; font-size: 24px; }}
                    .content {{ padding: 20px 0; line-height: 1.6; color: #4a4a4a; }}
                    .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #e0d5ce; font-size: 12px; color: #888; }}
                    .btn {{ display: inline-block; padding: 10px 20px; background-color: #b36a3a; color: #ffffff !important; text-decoration: none; border-radius: 5px; font-weight: bold; margin-top: 10px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Hearth & Harvest</h1>
                    </div>
                    <div class='content'>
                        {body}
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} Tiệm bánh Hearth & Harvest. Xin cảm ơn quý khách!</p>
                    </div>
                </div>
            </body>
            </html>";

            // If email settings are not configured, log and return (mock mode)
            if (string.IsNullOrEmpty(emailSettings["SmtpServer"]))
            {
                Console.WriteLine($"\n[EmailService Mock] To: {toEmail}\nSubject: {subject}\nBody: {body}\n");
                return;
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]!), true);
                await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
