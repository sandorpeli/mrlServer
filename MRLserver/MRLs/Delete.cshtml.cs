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
    public class DeleteModel : PageModel
    {
        private readonly MRLserver.Data.MRLservContext _context;

        public DeleteModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MRLmodel MRLmodel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mrlmodel = await _context.MRLmodel.FirstOrDefaultAsync(m => m.ID == id);

            if (mrlmodel == null)
            {
                return NotFound();
            }
            else
            {
                MRLmodel = mrlmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mrlmodel = await _context.MRLmodel.FindAsync(id);
            if (mrlmodel != null)
            {
                MRLmodel = mrlmodel;
                _context.MRLmodel.Remove(MRLmodel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
