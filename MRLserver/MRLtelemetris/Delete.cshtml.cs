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
    public class DeleteModel : PageModel
    {
        private readonly MRLserver.Data.MRLservContext _context;

        public DeleteModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mrltelemetrymodel = await _context.MRLtelemetryModel.FindAsync(id);
            if (mrltelemetrymodel != null)
            {
                MRLtelemetryModel = mrltelemetrymodel;
                _context.MRLtelemetryModel.Remove(MRLtelemetryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
