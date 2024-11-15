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

namespace MRLserver.MRLtelemetris
{
    public class EditModel : PageModel
    {
        private readonly MRLserver.Data.MRLservContext _context;

        public EditModel(MRLserver.Data.MRLservContext context)
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

            var mrltelemetrymodel =  await _context.MRLtelemetryModel.FirstOrDefaultAsync(m => m.ID == id);
            if (mrltelemetrymodel == null)
            {
                return NotFound();
            }
            MRLtelemetryModel = mrltelemetrymodel;
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

            _context.Attach(MRLtelemetryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MRLtelemetryModelExists(MRLtelemetryModel.ID))
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

        private bool MRLtelemetryModelExists(int id)
        {
            return _context.MRLtelemetryModel.Any(e => e.ID == id);
        }
    }
}
