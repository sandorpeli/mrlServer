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
    public class DetailsModel : PageModel
    {
        private readonly MRLserver.Data.MRLservContext _context;

        public DetailsModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

        public MRLtelemetryModel MRLtelemetryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mrltelemetrymodel = await _context.MRLtelemetryModel.FirstOrDefaultAsync(m => m.ID == id);
            if (mrltelemetrymodel == null)
            {
                return NotFound();
            }
            else
            {
                MRLtelemetryModel = mrltelemetrymodel;
            }
            return Page();
        }
    }
}
