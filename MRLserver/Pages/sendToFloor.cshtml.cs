using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MRLserver.Pages
{
    public class SendToFloorModel : PageModel
    {
        private readonly SharedMRLdata _sharedData;
        private readonly MRLserver.Data.MRLservContext _context;

        // Konstruktor injekcióval kapja meg a sharedData-t
        public SendToFloorModel(SharedMRLdata sharedData, MRLserver.Data.MRLservContext context)
        {
            _sharedData = sharedData;
            _context = context;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Handle different button actions
            string action = Request.Form["action"];
            switch (action)
            {
                case "button_sendtofloor":
                    string sendToFloorValue = Request.Form["sendtofloor"];
                    // Validate and process the value
                    if (int.TryParse(sendToFloorValue, out int sendToFloor) && sendToFloor >= 1 && sendToFloor <= 30)
                    {
                        _sharedData.SetData("313437303139510B00330027", "sendToFloor", (int)sendToFloor);
                        Console.WriteLine($"Floor number set to: {sendToFloor}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid floor number entered. Must be between 1 and 30.");
                    }
                    break;
                default:
                    break;
            }
            return RedirectToPage(); // Ez újra meghívja az OnGet metódust
            //return Page(); // Refresh the page to show updated output
        }
    }
}
