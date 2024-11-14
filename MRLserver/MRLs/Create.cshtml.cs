using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MRLserver.Data;
using MRLserver.Models;

namespace MRLserver.MRLs
{
    public class CreateModel : PageModel
    {
        private readonly MRLserver.Data.MRLserverContext _context;

        public CreateModel(MRLserver.Data.MRLserverContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public MRLclass MRLclass { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MRLclass.Add(MRLclass);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
