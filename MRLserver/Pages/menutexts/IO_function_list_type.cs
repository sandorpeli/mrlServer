namespace MRLserver.Pages.menutexts
{
    public class IO_function_list_type
    {
        // Property to store the list
        public List<IO_function_element> Items { get; private set; }

        // Constructor to initialize the list
        public IO_function_list_type()
        {
            Items = new List<IO_function_element>();
        }

        // Method to add an item to the list
        public void AddItem(IO_function_element item)
        {
            Items.Add(item);
        }

        // Method to remove an item from the list
        public bool RemoveItem(IO_function_element item)
        {
            return Items.Remove(item);
        }

        // Method to clear all items
        public void ClearItems()
        {
            Items.Clear();
        }
    }

    public class IO_function_element
    {
        public string ID { get; set; }
        public string Function { get; set; }
    }
}
