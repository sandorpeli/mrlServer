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
    public class DetailsModel : PageModel
    {
        private readonly MRLserver.Data.MRLserverContext _context;

        public DetailsModel(MRLserver.Data.MRLserverContext context)
        {
            _context = context;
        }

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
    }
}
