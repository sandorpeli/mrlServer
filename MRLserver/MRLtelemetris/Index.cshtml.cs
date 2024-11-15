using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MRLserver.Data;
using MRLserver.Models;

namespace MRLserver.MRLtelemetris
{
    public class IndexModel : PageModel
    {
        private readonly MRLserver.Data.MRLservContext _context;

        public IndexModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

        public IList<MRLtelemetryModel> MRLtelemetryModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            MRLtelemetryModel = await _context.MRLtelemetryModel.ToListAsync();
        }
    }
}
