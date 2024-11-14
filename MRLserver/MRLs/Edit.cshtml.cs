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
        private readonly MRLserver.Data.MRLservContext _context;

        public EditModel(MRLserver.Data.MRLservContext context)
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

            var mrlmodel =  await _context.MRLmodel.FirstOrDefaultAsync(m => m.ID == id);
            if (mrlmodel == null)
            {
                return NotFound();
            }
            MRLmodel = mrlmodel;
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

            _context.Attach(MRLmodel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MRLmodelExists(MRLmodel.ID))
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

        private bool MRLmodelExists(int id)
        {
            return _context.MRLmodel.Any(e => e.ID == id);
        }
    }
}
