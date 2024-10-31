using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationProvider.Models
{
    public class WelcomeRequestModel
    {
        public string Email { get; set; } = null!;

        public string? Name { get; set; }
    }
}
