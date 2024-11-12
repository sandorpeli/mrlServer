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
    }
}
