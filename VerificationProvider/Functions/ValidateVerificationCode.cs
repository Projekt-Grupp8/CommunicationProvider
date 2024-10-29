using Google.Protobuf.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class ValidateVerificationCode
    {
        private readonly ILogger<ValidateVerificationCode> _logger;
        private readonly IValidateVerificationCodeService _validateVerificationCodeService;

        public ValidateVerificationCode(ILogger<ValidateVerificationCode> logger, IValidateVerificationCodeService validateVerificationCodeService)
        {
            _logger = logger;
            _validateVerificationCodeService = validateVerificationCodeService;
        }

        [Function("ValidateVerificationCode")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post")] HttpRequest req)
        {
            try
            {
                var validateRequest = await _validateVerificationCodeService.UnpackValidateRequestAsync(req);
                if(validateRequest != null)
                {
                    bool validateResult = await _validateVerificationCodeService.ValidateCodeAsync(validateRequest);

                    if (validateResult)
                    {
                        return new OkResult();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: ValidateVerificationCode.Run :: {ex.Message}");
            }

            return new UnauthorizedResult();
        }

       
    }
}
