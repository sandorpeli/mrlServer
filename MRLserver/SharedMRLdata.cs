using System.Collections.Concurrent;

namespace MRLserver
{
    public class SharedMRLdata
    {
        // Minden string-hez egy Dictionary-t tárolunk kulcs-érték párokkal
        private ConcurrentDictionary<string, Dictionary<string, object>> data = new ConcurrentDictionary<string, Dictionary<string, object>>();

        // Adat hozzáadása a dictionary-hez egy string kulccsal
        public void SetData(string key, string subKey, object value)
        {
            if (!data.ContainsKey(key))
            {
                data[key] = new Dictionary<string, object>();
            }
            data[key][subKey] = value;
        }

        // Adat lekérdezése a dictionary-ból
        public object GetData(string key, string subKey)
        {
            if (data.ContainsKey(key) && data[key].ContainsKey(subKey))
            {
                return data[key][subKey];
            }
            return null; // Vagy dobj kivételt, ha nem található
        }

        // Az összes adat lekérdezése egyedi azonosító alapján (opcionális, ha szükséges)
        public Dictionary<string, object> GetAllData(string id)
        {
            if (data.TryGetValue(id, out var dict))
            {
                return dict;
            }
            else
            {
                throw new KeyNotFoundException($"A(z) {id} azonosítóval rendelkező adat nem található.");
            }
        }

        // Delete a specific subKey from a given key
        public bool DeleteSubKey(string key, string subKey)
        {
            if (data.TryGetValue(key, out var subDict))
            {
                if (subDict.Remove(subKey))
                {
                    // If the dictionary is now empty, optionally remove the top-level key
                    if (subDict.Count == 0)
                    {
                        data.TryRemove(key, out _);
                    }
                    return true;
                }
            }
            return false; // Return false if key or subKey not found
        }

        // Delete the entire top-level key
        public bool DeleteKey(string key)
        {
            return data.TryRemove(key, out _);
        }

    }
}
