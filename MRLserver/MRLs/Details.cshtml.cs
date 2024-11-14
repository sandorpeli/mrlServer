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
        private readonly MRLserver.Data.MRLservContext _context;

        public DetailsModel(MRLserver.Data.MRLservContext context)
        {
            _context = context;
        }

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
    }
}
