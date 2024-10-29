using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VerificationProvider.Data.Contexts;


namespace VerificationProvider.Services
{
    public class VerificationCleanerService : IVerificationCleanerService
    {
        private readonly ILogger<VerificationCleanerService> _logger;
        private readonly DatabaseContexts _Contexts;

        public VerificationCleanerService(ILogger<VerificationCleanerService> logger, DatabaseContexts contexts)
        {
            _logger = logger;
            _Contexts = contexts;
        }

        public async Task RemoveExpiredRecordsAsync()
        {
            try
            {
                
                var expiredRequests = await _Contexts.VerificationRequests.Where(x => x.ExpiryDate <= DateTime.UtcNow).ToListAsync();
                if(expiredRequests.Any())
                {
                    _Contexts.RemoveRange(expiredRequests);
                    await _Contexts.SaveChangesAsync();
                    _logger.LogInformation($"{expiredRequests.Count} utgångna verifieringsförfrågningar togs bort.");
                }
                else
                {
                    _logger.LogInformation("Det fanns inget i listan att rensa. Tiden");
                    _logger.LogInformation($"System Local Time: {DateTime.Now}, UTC Time: {DateTime.UtcNow}");
                    return;
                }
                
                 
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: VerificationCleanerService.RemoveExpiredRecordsAsync :: {ex.Message}");
            }
        }
    }

}
