using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MRLserver.Pages
{
    public class manufacturer_system_menuModel : PageModel
    {
        private readonly SharedMRLdata _sharedData;

        public manufacturer_system_menuModel(SharedMRLdata sharedData)
        {
            _sharedData = sharedData;
        }

        public void OnGet(SharedMRLdata sharedData)
        {
            _sharedData.SetData("313437303139510B00330027", "GetAllData", 1);
        }

        public IActionResult OnPost()
        {
            var action = Request.Form["action"];

            switch (action)
            {
                case "Kimenet":
                    return RedirectToPage("/Menus/setOutputPin");
                case "Bemenet":
                    // Handle Bemenet v�laszt�s
                    break;
                case "ModulIO":
                    // Handle Modul I/O v�laszt�s
                    break;
                case "CDP":
                    // Handle CDP funkci� v�laszt�s
                    break;
            }

            return Page(); // Or RedirectToPage() as needed
        }
    }
}
