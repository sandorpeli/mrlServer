using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MRLserver.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SharedMRLdata _sharedData;
        private readonly ILogger<IndexModel> _logger;

        public string ProgramOutput { get; set; }

        // Konstruktor injekcióval kapja meg a sharedData-t
        public IndexModel(SharedMRLdata sharedData)
        {
            _sharedData = sharedData;
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
            }
        }
    }
}
