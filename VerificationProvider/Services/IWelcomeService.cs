using Azure.Messaging.ServiceBus;
using VerificationProvider.Models;

namespace VerificationProvider.Services
{
    public interface IWelcomeService
    {
        string GenerateServiceBusEmailRequest(EmailRequest emailRequest);
        EmailRequest GenerateWelcomeEmailRequest(string email, string name);
        WelcomeRequestModel UnPackWelcomeRequest(ServiceBusReceivedMessage message);
    }
}