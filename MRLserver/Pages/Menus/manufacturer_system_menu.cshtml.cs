using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MRLserver.Pages
{
    public class manufacturer_system_menuModel : PageModel {
        private readonly SharedMRLdata _sharedData;
        private bool FIRST_TIME = true;
        private static bool _backgroundTaskCompleted = false;

        public manufacturer_system_menuModel(SharedMRLdata sharedData) {
            _sharedData = sharedData;
            _backgroundTaskCompleted = false;
        }

        public void OnGet(SharedMRLdata sharedData) {
            _sharedData.SetData("313437303139510B00330027", "GetAllData", 1);

            // Show modal initially when OnGet is called
            if (FIRST_TIME) {
                TempData["ShowModal"] = false;      // TODO nem sikerült megoldani, hogy az adatlekérés után eltûnjön a modal.
                FIRST_TIME = false;
            } else {
                TempData["ShowModal"] = false;  // Set it to false after first time
            }

            // Start background task
            if (!_backgroundTaskCompleted) {     
                _ = Task.Run(async () =>
                {
                    await WaitForAllDatatRefresh();
                    _backgroundTaskCompleted = true;

                    TempData["ShowModal"] = false;

                    //RedirectToPage("/Menus/setOutputPin");
                    Response.Redirect(Request.Path);  // Reload the current page
                    //RedirectToPage();

                    //Response.Redirect("/Menus/manufacturer_system_menu");
                });

            }            
        }

        public IActionResult OnPost() {
            var action = Request.Form["action"];

            switch (action) {
                case "Kimenet":
                    return RedirectToPage("/Menus/setOutputPin");
                case "Bemenet":
                    // Handle Bemenet választás
                    break;
                case "ModulIO":
                    // Handle Modul I/O választás
                    break;
                case "CDP":
                    // Handle CDP funkció választás
                    break;
            }

            return Page(); // Or RedirectToPage() as needed
        }

        private async Task WaitForAllDatatRefresh() {
            bool excape_flag = false;
            int cnt = 0;

            while (excape_flag == false) {
                await Task.Delay(100);

                var data = _sharedData.GetData("313437303139510B00330027", "GetAllDataInProgress");
                var data2 = _sharedData.GetData("313437303139510B00330027", "GetAllData");

                
                if ((data == null || (data is bool && (bool)data == false)) && (data2 == null || (data2 is int && (int)data2 == 0))) {
                    // Do something when data is either null or false
                    excape_flag = true;
                } else {
                    cnt++;
                    if (cnt >= 100) {
                        excape_flag = true;
                    }
                }
            }
        }
    }
}