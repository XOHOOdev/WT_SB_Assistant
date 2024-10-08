using Newtonsoft.Json.Linq;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

namespace WtSbAssistant.Core.Converters
{
    public class ConfigConverter
    {
        public static Configuration[] Deserialize(string json)
        {
            List<Configuration> configs = [];

            var jsonObject = JObject.Parse(json);
            configs.AddRange(from item in jsonObject.Children() from item2 in item.First().Children() let prop = item2 as JProperty select new Configuration { Class = item.Path, Property = prop?.Name ?? "", Value = prop?.Value.ToString() ?? "" });

            return configs.ToArray();
        }

        public static string Serialize(Configuration[] dbConfigurations)
        {
            JObject jsonObject = new();
            foreach (var config in dbConfigurations)
            {
                if (jsonObject.Children().FirstOrDefault(x => x.Path == config.Class) is not JProperty item)
                {
                    item = new JProperty(config.Class, new JObject());
                    jsonObject.Add(item);
                }
                ((JObject)item.Value).Add(new JProperty(config.Property, config.Value));
            }
            return jsonObject.ToString();
        }
    }
}
