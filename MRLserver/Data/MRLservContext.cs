using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MRLserver.Models;

namespace MRLserver.Data
{
    public class MRLservContext : DbContext
    {
        public MRLservContext (DbContextOptions<MRLservContext> options)
            : base(options)
        {
        }

        public DbSet<MRLserver.Models.MRLmodel> MRLmodel { get; set; } = default!;
    }
}
