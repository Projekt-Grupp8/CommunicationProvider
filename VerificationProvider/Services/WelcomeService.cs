using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Models;

namespace VerificationProvider.Services
{
    public class WelcomeService : IWelcomeService
    {
        private readonly ILogger<WelcomeService> _logger;

        public WelcomeService(ILogger<WelcomeService> logger)
        {
            _logger = logger;
        }

        public EmailRequest GenerateWelcomeEmailRequest(string email, string name)
        {
            try
            {
                var emailRequest = new EmailRequest
                {
                    To = email,
                    Subject = "Välkommen till Rika!",
                    HtmlBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; color: #333;'>
                            <h2>Välkommen till Rika!</h2>
                            <p>Hej {name},</p>
                            <p>Tack för att du har gått med i vår community! Vi är glada att ha dig här.</p>
                            <p>Hos oss hittar du massor av resurser och bra produkter. Logga in för att komma igång!</p>
                            <br/>
                            <p>Vänliga hälsningar,<br/>Rika-teamet</p>
                        </body>
                    </html>",
                    PlainText = $"Välkommen till Rika!\n\nHej {name},\n\nTack för att du har gått med i vår community! Vi är glada att ha dig här.\n\nVänliga hälsningar,\nRika-teamet"
                };

                return emailRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: GenerateWelcomeEmailRequest.GenerateEmailRequest.Run :: {ex.Message}");
            }
            return null!;
        }

        public WelcomeRequestModel UnPackWelcomeRequest(ServiceBusReceivedMessage message)
        {
            try
            {
                var welcomeRequest = JsonConvert.DeserializeObject<WelcomeRequestModel>(message.Body.ToString());

                if (welcomeRequest != null && !string.IsNullOrEmpty(welcomeRequest.Email))
                {
                    return welcomeRequest!;
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: WelcomeReq.UnPackVerificationRequest :: {ex.Message}");
            }
            return null!;

        }

        public string GenerateServiceBusEmailRequest(EmailRequest emailRequest)
        {
            try
            {
                var data = JsonConvert.SerializeObject(emailRequest);

                if (!string.IsNullOrEmpty(data))
                {
                    return data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: WelcomeRequest.GenerateServiceBusEmailRequest :: {ex.Message}");
            }
            return null!;
        }

    }
}
