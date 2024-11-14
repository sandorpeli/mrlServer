using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MRLserver.Models;
using System.Collections.Generic;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MRLserver.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SharedMRLdata _sharedData;
        private readonly ILogger<IndexModel> _logger;
        private readonly MRLserver.Data.MRLservContext _context;

        public string ProgramOutput { get; set; }
        public string telepitesHelye { get; set; }
        public string UID { get; set; }
        public string telepitesPozicioja { get; set; }
        public string telepitestVegezte { get; set; }
        public string telepitesIdeje { get; set; }
        public string karbantarto { get; set; }
        public string utolsoKarbantartasIdeje { get; set; }
        public string kovetkezoKarbantartas { get; set; }
        public string utolsoKapcsolataLifttel { get; set; }


        // Konstruktor injekcióval kapja meg a sharedData-t
        public IndexModel(SharedMRLdata sharedData, MRLserver.Data.MRLservContext context)
        {
            _sharedData = sharedData;
            telepitesHelye = "Z telephely";
            _context = context;
        }
/*
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
*/
        public void OnGet()
        {
            string myID = (string)_sharedData.GetData("313437303139510B00330027", "myID");

            if(myID != null && myID.Length > 3)
            {
                DateTime TimeStamp = (DateTime)_sharedData.GetData("313437303139510B00330027", "TimeStamp");
                int DoorStateA = (int)_sharedData.GetData("313437303139510B00330027", "DoorStateA"); 
                int DoorStateB = (int)_sharedData.GetData("313437303139510B00330027", "DoorStateB"); 
                int ElevatorState = (int)_sharedData.GetData("313437303139510B00330027", "ElevatorState"); 
                int Travel1 = (int)_sharedData.GetData("313437303139510B00330027", "Travel1"); 
                int Travel2 = (int)_sharedData.GetData("313437303139510B00330027", "Travel2");
                int[] VVVFErrors = new int[5];
                VVVFErrors = (int[])_sharedData.GetData("313437303139510B00330027", "VVVFErrors"); 
                string Errors = (string)_sharedData.GetData("313437303139510B00330027", "Errors");

                string fullText = myID;
                fullText = fullText + "\n" + TimeStamp.ToString("yyyy.MM.dd HH:mm:ss") + "\n" + DoorStateA + "\n" + DoorStateB + "\n" + ElevatorState + "\n" + Travel1 + ", " + Travel2 + "\n" + string.Join(",", VVVFErrors) + "\n" + Errors;
                ProgramOutput = fullText;

                var mrlmodel = _context.MRLmodel
                    .Where(m => m.UID == "313437303139510B00330027")
                    .OrderByDescending(m => m.ID) // assuming 'Id' is the primary key or a field for ordering
                    .FirstOrDefault();

                if (mrlmodel != null)
                {
                    if (mrlmodel.utolsoKapcsolataLifttel.HasValue) {
                        utolsoKapcsolataLifttel = mrlmodel.utolsoKapcsolataLifttel.ToString();
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            // Handle different button actions
            string action = Request.Form["action"];
            switch (action)
            {
                case "button1":
                    OnGet();
                    //ProgramOutput = "Button 1 clicked";
                    break;
                case "button2":
                    ProgramOutput = "Button 2 clicked";
                    break;
                case "button3":
                    ProgramOutput = "Button 3 clicked";
                    break;
                default:
                    ProgramOutput = "No action selected";
                    break;
            }
            return RedirectToPage(); // Ez újra meghívja az OnGet metódust
            //return Page(); // Refresh the page to show updated output
        }
    }
}
