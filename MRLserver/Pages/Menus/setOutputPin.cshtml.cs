using CsvHelper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MRLserver.Pages.menutexts;
using System.Drawing;
using System.Globalization;
using System.Security.Policy;

namespace MRLserver.Pages.Menus
{
    public class setOutputPinModel : PageModel
    {

        public csv_text_data _csvTextData;
        public List<string> OutputName { get; private set; } = new List<string>();


        public IO_function_list_type myFunctionList = new IO_function_list_type();

        // Konstruktor
        public setOutputPinModel(csv_text_data csvTextData)
        {
            for (int i = 1; i <= 14; i++)
            {
                string tmp_name = "Relé " + i.ToString(); 
                OutputName.Add(tmp_name);
            }
            for (int i = 1; i <= 8; i++)
            {
                string tmp_name = "NPN " + i.ToString();
                OutputName.Add(tmp_name);
            }

            _csvTextData = csvTextData;
        }

        public void OnGet()
        {

        }




    }

}

