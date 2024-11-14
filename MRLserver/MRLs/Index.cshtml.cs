using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MRLserver.Data;
using MRLserver.Models;

namespace MRLserver.MRLs
{
    public class IndexModel : PageModel
    {
        private readonly MRLserver.Data.MRLserverContext _context;

        public IndexModel(MRLserver.Data.MRLserverContext context)
        {
            _context = context;
        }

        public IList<MRLclass> MRLclass { get;set; } = default!;

        public async Task OnGetAsync()
        {
            MRLclass = await _context.MRLclass.ToListAsync();
        }
    }
}
