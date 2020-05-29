using Newtonsoft.Json;
using DataLayer.Entities;
using System.IO;

namespace ServiceLayer.Services
{
    public class ConfigService
    {
        public Config GetConfig()
        {
            var file = "Config.json";
            var data = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<Config>(data);
        }
    }
}
