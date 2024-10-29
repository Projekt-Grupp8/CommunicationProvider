using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Functions;
using VerificationProvider.Models;

namespace VerificationProvider.Services;

public class VerificationService(IServiceProvider serviceProvider, ILogger<VerificationService> logger) : IVerificationService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<VerificationService> _logger = logger;


    public async Task<bool> SaveVerificationRequest(VerificationRequest verificationRequest, string code)
    {
        try
        {
            using var context = _serviceProvider.GetRequiredService<DatabaseContexts>();

            var existingRequest = await context.VerificationRequests.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);

            if (existingRequest != null)
            {
                existingRequest.Code = code;
                existingRequest.ExpiryDate = DateTime.UtcNow.AddMinutes(5);
                context.Entry(existingRequest).State = EntityState.Modified;
            }
            else
            {
                context.VerificationRequests.Add(new Data.Entities.VerificationRequestEntity { Email = verificationRequest.Email, Code = code });
            }
            await context.SaveChangesAsync();

            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR: SaveVerificationRequest.Run :: {ex.Message}");
        }
        return false;
    }

    public EmailRequest GenerateEmailRequest(string email, string code)
    {
        try
        {
            var emailRequest = new EmailRequest
            {
                To = email,
                Subject = $"Your Verification Code:{code}",
                HtmlBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; color: #333;'>
                            <h2>Hello,</h2>
                            <p>Thank you for using our service. Here is your verification code:</p>
                            <div style='font-size: 24px; font-weight: bold; color: #2d89ef; margin: 10px 0;'>{code}</div>
                            <p>Please enter this code within the next 10 minutes to complete your verification.</p>
                            <br/>
                            <p>Best regards,<br/>The Verification Team</p>
                            <p>Your emailrequest: {email}</p>
                        </body>
                    </html>",
                PlainText = $"Hello,\n\nYour verification code is: {code}\n\nPlease enter this code within the next 10 minutes to complete your verification.\n\nBest regards,\nThe Verification Team"
            };

            return emailRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR: GenerateVerificationCode.GenerateEmailRequest.Run :: {ex.Message}");
        }
        return null!;
    }

    public VerificationRequest UnPackVerificationRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var VerificationRequest = JsonConvert.DeserializeObject<VerificationRequest>(message.Body.ToString());

            if (VerificationRequest != null && !string.IsNullOrEmpty(VerificationRequest.Email))
            {
                return VerificationRequest;
            }
            else
            {
                return null!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR: GenerateVerificationCode.UnPackVerificationRequest :: {ex.Message}");
        }
        return null!;

    }

    public string GenerateCode()
    {
        try
        {
            var code = new Random().Next(100000, 999999);
            return code.ToString();

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR: GenerateVerificationCode.GenerateCode :: {ex.Message}");
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
            _logger.LogError($"ERROR: GenerateVerificationCode.GenerateServiceBusEmailRequest :: {ex.Message}");
        }
        return null!;
    }
}
