using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class WelcomeRequest(ILogger<WelcomeRequest> logger, IWelcomeService welcomeService)
    {
        private readonly ILogger<WelcomeRequest> _logger = logger;
        private readonly IWelcomeService _welcomeService = welcomeService;

        [ServiceBusOutput("email_request", Connection = "ServiceBusConnection")]
        [Function(nameof(WelcomeRequest))]
        public async Task<string> Run([ServiceBusTrigger("welcome_request", Connection = "ServiceBusConnection")]ServiceBusReceivedMessage message,ServiceBusMessageActions messageActions)
        {
            try
            {
                var welcomeRequest = _welcomeService.UnPackWelcomeRequest(message);
                if (welcomeRequest != null)
                {
                    var emailRequest = _welcomeService.GenerateWelcomeEmailRequest(welcomeRequest.Email, welcomeRequest.Name!);

                    if (emailRequest != null)
                    {
                        var data = _welcomeService.GenerateServiceBusEmailRequest(emailRequest);
                        if (!string.IsNullOrEmpty(data))
                        {
                            await messageActions.CompleteMessageAsync(message);
                            _logger.LogInformation($"Message sent succesfully by:{welcomeRequest.Email}");
                            return data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: WelcomeRequest.Run :: {ex.Message}");
            }

            return null!;
           
            
        }
        

    }
}
