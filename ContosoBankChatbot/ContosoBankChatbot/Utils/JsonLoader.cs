using ContosoBankChatbot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using System.Reflection;

namespace ContosoBankChatbot.Utils
{
    public class JsonLoader
    {
        public static Dictionary<string, CurrencyModel> LoadJsonToCurrency(string jsonFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(jsonFileName))
            using (StreamReader r = new StreamReader(stream))
            {
                string json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<Dictionary<string, CurrencyModel>>(json);
                return items;
            }
        }

    }
}