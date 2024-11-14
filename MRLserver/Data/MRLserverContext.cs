using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MRLserver.Models;

namespace MRLserver.Data
{
    public class MRLserverContext : DbContext
    {
        public MRLserverContext (DbContextOptions<MRLserverContext> options)
            : base(options)
        {
        }

        public DbSet<MRLserver.Models.MRLclass> MRLclass { get; set; } = default!;
    }
}
