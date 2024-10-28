using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerificationProvider.Data.Entities;

namespace VerificationProvider.Data.Contexts
{
    public class DatabaseContexts(DbContextOptions<DatabaseContexts> options) : DbContext(options)
    {
        public DbSet<VerificationRequestEntity> VerificationRequests { get; set; }
    }
}
