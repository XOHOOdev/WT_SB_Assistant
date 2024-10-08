using Microsoft.Extensions.Configuration;

namespace WtSbAssistant.Core.Helpers
{
    public class ConfigLoader
    {
        public static IConfigurationRoot Load(Env? env = null)
        {
            var configurationBuilder = new ConfigurationBuilder();
            AddJsonFiles(configurationBuilder, env);

            return configurationBuilder.Build();
        }


        public static void AddJsonFiles(IConfigurationBuilder configurationBuilder, Env? env = null)
        {
            if (!env.HasValue)
                env = EnvHelper.GetEnvironment();

            configurationBuilder
                             .AddJsonFile($"Config/appsettings.json")
                             .AddJsonFile($"Config/appsettings.{env}.json");
        }
    }
}
