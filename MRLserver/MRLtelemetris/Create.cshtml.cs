using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MRLserver.Data;
using MRLserver.Models;

namespace MRLserver.MRLtelemetris
{
    public class CreateModel : PageModel
    {
        private readonly MRLserver.Data.MRLservContext _context;

        public CreateModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public MRLtelemetryModel MRLtelemetryModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MRLtelemetryModel.Add(MRLtelemetryModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
