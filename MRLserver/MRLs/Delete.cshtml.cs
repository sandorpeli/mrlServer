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
        private readonly MRLserver.Data.MRLserverContext _context;

        public DeleteModel(MRLserver.Data.MRLserverContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MRLclass MRLclass { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mrlclass = await _context.MRLclass.FirstOrDefaultAsync(m => m.ID == id);

            if (mrlclass == null)
            {
                return NotFound();
            }
            else
            {
                MRLclass = mrlclass;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mrlclass = await _context.MRLclass.FindAsync(id);
            if (mrlclass != null)
            {
                MRLclass = mrlclass;
                _context.MRLclass.Remove(MRLclass);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
