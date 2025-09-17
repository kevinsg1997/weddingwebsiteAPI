using SendGrid;
using SendGrid.Helpers.Mail;

namespace WeddingMerchantApi.Services
{
    public class EmailService
    {
        private readonly string _apiKey;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SENDGRID_API_KEY"]
                    ?? throw new ArgumentNullException("SENDGRID_API_KEY", "A chave da API do SendGrid não foi configurada.");
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("kevinsg1997@gmail.com", "Convite: Casamento Kevin & Pamela");
            var tos = new List<EmailAddress>
            {
                new EmailAddress("kevinsg1997@gmail.com"),
                new EmailAddress("pamelarafa99@gmail.com")
            };
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent: "", htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}