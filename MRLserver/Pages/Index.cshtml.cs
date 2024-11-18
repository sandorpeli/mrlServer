using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MRLserver.Models;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MRLserver.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SharedMRLdata _sharedData;
        private readonly ILogger<IndexModel> _logger;
        private readonly MRLserver.Data.MRLservContext _context;


        private int telemetry_record_shift;

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
        public string DoorA { get; set; }
        public string DoorB { get; set; }
        public string Travels { get; set; }
        public string VVVFErrors_s { get; set; }
        public string Errors { get; set; }
        public string RecordDate { get; set; }
        public string Records { get; set; }

        



        // Konstruktor injekcióval kapja meg a sharedData-t
        public IndexModel(SharedMRLdata sharedData, MRLserver.Data.MRLservContext context)
        {
            _sharedData = sharedData;
            _context = context;

            if (_sharedData != null)
            {
                var data = _sharedData.GetData("InternalData", "telemetry_record_shift");
                if (data != null)
                {
                    telemetry_record_shift = (int)data;
                }
                else
                {
                    telemetry_record_shift = 0;
                }
            }
            else
            {
                telemetry_record_shift = 0;
            }
        }
/*
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
*/
        public void OnGet()
        {
            string myID = "";
            if (_sharedData != null)
            {
                var data = _sharedData.GetData("313437303139510B00330027", "myID");
                if (data != null)
                {
                    myID = (string)data;
                }
            }
           // string myID = (string)_sharedData.GetData("313437303139510B00330027", "myID");

            if (myID != null && myID.Length > 3)
            {
                var data = _sharedData.GetData("313437303139510B00330027", "myID");

                if (myID != null && myID.Length > 3)
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



            var mrlTelemetry = _context.MRLtelemetryModel
                .Where(m => m.UID == "313437303139510B00330027")
                .OrderByDescending(m => m.ID) // assuming 'Id' is the primary key or a field for ordering
                .Skip(telemetry_record_shift)
                .FirstOrDefault();

            Console.WriteLine("telemetry_record_shift: " + telemetry_record_shift);

            if (mrlTelemetry != null)
            {
                if (mrlTelemetry.utolsoKapcsolataLifttel.HasValue)
                {
                    DoorA = mrlTelemetry.DoorStateA.ToString();
                    DoorB = mrlTelemetry.DoorStateB.ToString();
                    Travels = mrlTelemetry.Travel1.ToString() + " / " + mrlTelemetry.Travel2.ToString();
                    VVVFErrors_s = mrlTelemetry.VVVFErrors;
                    Errors = mrlTelemetry.Errors;
                    RecordDate = mrlTelemetry.utolsoKapcsolataLifttel.ToString();

                    int count = _context.MRLtelemetryModel.Count(m => m.UID == "313437303139510B00330027");
                    Records = (count- telemetry_record_shift).ToString()  + " / " + count.ToString();
                }
            }

            var mrlModel = _context.MRLmodel
                .Where(m => m.UID == "313437303139510B00330027")
                .OrderByDescending(m => m.ID) // assuming 'Id' is the primary key or a field for ordering
                .FirstOrDefault();

            Console.WriteLine("telemetry_record_shift: " + telemetry_record_shift);

            if (mrlModel != null)
            {
                if (!string.IsNullOrEmpty(mrlModel.UID))
                {
                    utolsoKapcsolataLifttel = mrlModel.utolsoKapcsolataLifttel?.ToString() ?? "";
                    telepitesHelye = mrlModel.telepitesHelye?.ToString() ?? "";
                    telepitestVegezte = mrlModel.telepitestVegezte?.ToString() ?? "";
                    telepitesPozicioja = mrlModel.telepitesPozicioja?.ToString() ?? "";
                    telepitesIdeje = mrlModel.telepitesIdeje?.ToString() ?? "";
                    utolsoKarbantartasIdeje = mrlModel.utolsoKarbantartasIdeje?.ToString() ?? "";
                    kovetkezoKarbantartas = mrlModel.kovetkezoKarbantartas?.ToString() ?? "";
                    UID = mrlModel.UID.ToString();
                    karbantarto = mrlModel.karbantarto?.ToString() ?? "";
                }
            }

        }

        public IActionResult OnPost()
        {

            // Handle different button actions
            string action = Request.Form["action"];
            switch (action)
            {
                case "button_refresh":
                    OnGet();
                    //SendToApi.SendMessageAsync("Kilroy was here").GetAwaiter().GetResult();
                    break;
                case "button_formerly":
                    int rowCount = _context.MRLtelemetryModel
                       .Where(m => m.UID == "313437303139510B00330027")
                       .Count();

                    if (rowCount > 0 && (rowCount > (telemetry_record_shift + 1)))
                    {
                        telemetry_record_shift++;
                        _sharedData.SetData("InternalData", "telemetry_record_shift", telemetry_record_shift);
                        Console.WriteLine("telemetry_record_shift++: " + telemetry_record_shift);
                        //OnGet();
                    }
                    break;
                case "button_next":
                    rowCount = _context.MRLtelemetryModel
                       .Where(m => m.UID == "313437303139510B00330027")
                       .Count();

                    if (rowCount > 0 && telemetry_record_shift > 0)
                    {
                        telemetry_record_shift--;
                        _sharedData.SetData("InternalData", "telemetry_record_shift", telemetry_record_shift);
                        Console.WriteLine("telemetry_record_shift--: " + telemetry_record_shift);
                        //OnGet();
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
