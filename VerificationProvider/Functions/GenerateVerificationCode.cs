using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerificationProvider.Functions
{
    public class GenerateVerificationCode
    {
        private readonly ILogger<GenerateVerificationCode> _logger;
        

        public GenerateVerificationCode(ILogger<GenerateVerificationCode> logger)
        {
            _logger = logger;
        }
        
        [Function(nameof(GenerateVerificationCode))]
        [ServiceBusOutput("email_request", Connection = "ServiceBusConnection")]
        public async Task Run([ServiceBusTrigger("verification_request", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,ServiceBusMessageActions messageActions)
        {
            
        }
    }
}
