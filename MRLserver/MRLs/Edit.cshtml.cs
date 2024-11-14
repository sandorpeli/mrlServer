using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MRLserver.Data;
using MRLserver.Models;

namespace MRLserver.MRLs
{
    public class EditModel : PageModel
    {
        private readonly MRLserver.Data.MRLserverContext _context;

        public EditModel(MRLserver.Data.MRLserverContext context)
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

            var mrlclass =  await _context.MRLclass.FirstOrDefaultAsync(m => m.ID == id);
            if (mrlclass == null)
            {
                return NotFound();
            }
            MRLclass = mrlclass;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MRLclass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MRLclassExists(MRLclass.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MRLclassExists(int id)
        {
            return _context.MRLclass.Any(e => e.ID == id);
        }
    }
}
