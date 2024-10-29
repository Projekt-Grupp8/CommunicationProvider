using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class GenerateVerificationCode(ILogger<GenerateVerificationCode> logger, IVerificationService service)
    {
        private readonly ILogger<GenerateVerificationCode> _logger = logger;
        private readonly IVerificationService _verificationService = service;
        

        [Function(nameof(GenerateVerificationCode))]
        [ServiceBusOutput("email_request", Connection = "ServiceBusConnection")]
        public async Task<string> Run([ServiceBusTrigger("verification_request", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,ServiceBusMessageActions messageActions)
        {
            try
            {
                var VerificationRequest = _verificationService.UnPackVerificationRequest(message);
                if (VerificationRequest != null)
                {
                    var Code = _verificationService.GenerateCode();
                    
                    if(Code != null && !string.IsNullOrEmpty(Code))
                    {
                        var result = await _verificationService.SaveVerificationRequest(VerificationRequest, Code);
                        if (result)
                        {
                            var emailRequest = _verificationService.GenerateEmailRequest(VerificationRequest.Email, Code);
                            if (emailRequest != null)
                            {
                                var payLoad = _verificationService.GenerateServiceBusEmailRequest(emailRequest);
                                if (!string.IsNullOrEmpty(payLoad))
                                {
                                    await messageActions.CompleteMessageAsync(message);
                                    return payLoad;
                                }
                            }
                        }

                       
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: GenerateVerificationCode.Run :: {ex.Message}");
            }

            return null!;
        }

       


    }
}
