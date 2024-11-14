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
        private readonly MRLserver.Data.MRLservContext _context;

        public IndexModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

        public IList<MRLmodel> MRLmodel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            MRLmodel = await _context.MRLmodel.ToListAsync();
        }
    }
}
