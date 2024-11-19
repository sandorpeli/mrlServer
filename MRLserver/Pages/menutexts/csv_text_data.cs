namespace MRLserver.Pages.menutexts
{
    public class csv_text_data
    {
        public IO_function_list_type OutputFunctionList = new IO_function_list_type();

        public csv_text_data()
        {
            ReadCsv();
        }

        private void ReadCsv()
        {
            bool foundStart = false;
            int rowsToProcess = 0;
            int currentRow = 0;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\MRLserver\Pages\menutexts\menutext.csv");

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var rowText = reader.ReadLine();

                    // Check if we've found the start of the relevant data
                    if (!foundStart && rowText.Contains("IO_functions_Output_text"))
                    {
                        // Split the row by pipe ('|') and try to extract the number of rows to process
                        var parts = rowText.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                        // Ensure the number is in the expected position (after "IO_functions_Output_text")
                        if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out rowsToProcess))
                        {
                            foundStart = true; // Mark that we've found the start of interesting data
                            Console.WriteLine($"Found the start. Rows to process: {rowsToProcess}");
                        }
                    }
                    else if (foundStart)
                    {
                        // Process the rows after finding the start, based on the extracted number
                        // For example, read and store ID and Function values for the relevant rows
                        if (currentRow < rowsToProcess)
                        {
                            // Assuming the CSV has the following format: ID|Function|OtherData|MoreData
                            var columns = rowText.Split('|');
                            if (columns.Length >= 2) // Ensure we have enough columns
                            {
                                var entry = new IO_function_element
                                {
                                    ID = columns[0].Trim(), // Extracting ID (first column)
                                    Function = columns[1].Trim() // Extracting Function (second column)
                                };

                                OutputFunctionList.AddItem(entry); // Add the entry to the list
                                currentRow++; // Increment the row counter
                            }
                        }
                        else
                        {
                            // Stop processing after we reach the specified number of rows
                            break;
                        }
                    }
                }
            }

        }
    }
}
