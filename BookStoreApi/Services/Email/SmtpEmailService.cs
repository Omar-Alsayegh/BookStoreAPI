using BookStoreApi.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace BookStoreApi.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message,ICollection<string> bccEmails)
        {
            
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpClient = new SmtpClient(smtpSettings["Host"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"]),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);
            if (bccEmails != null && bccEmails.Any()) // Ensure the list is not null and has emails
            {
                foreach (var bccEmail in bccEmails)
                {
                    mailMessage.Bcc.Add(bccEmail); // Add each email to the Bcc collection
                }
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
