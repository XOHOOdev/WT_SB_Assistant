using Microsoft.AspNetCore.Identity;
using WtSbAssistant.Core.Converters;
using WtSbAssistant.Core.DataAccess.DatabaseAccess;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

/**
 * Update Migration
 * Scaffold-DbContext "Server=localhost,1434;Database=SalesDb;User Id=SA;Password=A&VeryComplex123Password;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force
 */

namespace WtSbAssistant.Core.Helpers
{
    public class ConfigHelper(ApplicationDbContext<IdentityUser, ApplicationRole, string> context)
    {
        public void SetConfig(string json)
        {
            if (context.CF_Configurations.Any()) return;

            foreach (var config in ConfigConverter.Deserialize(json))
            {
                var reqObj = context.Find(typeof(Configuration), [config.Class, config.Property]);
                if (reqObj != null)
                {
                    ((Configuration)reqObj).Value = config.Value;
                    continue;
                }
                context.Add(config);
            }
            context.SaveChanges();
        }

        public string? GetConfig(string className, string config)
        {
            return (context.Find(typeof(Configuration), [className, config]) as Configuration)?.Value;
        }

        public string GetConfigAsJson()
        {
            return ConfigConverter.Serialize(context.CF_Configurations.ToArray());
        }
    }
}