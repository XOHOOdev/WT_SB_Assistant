using Microsoft.AspNetCore.Identity;
using Sparta.BlazorUI.Authorization;
using Sparta.Core.DataAccess.DatabaseAccess;
using Sparta.Core.DataAccess.DatabaseAccess.Entities;
using Sparta.Core.Helpers;

namespace Sparta.BlazorUI.Data.ConfigurationData;

[HasPermission(Permissions.Permissions.Configuration.View)]
public class ConfigurationService(ApplicationDbContext<IdentityUser, ApplicationRole, string> context, ConfigHelper config)
{
    public ConfigurationCategory[] GetConfigurations()
    {
        var dbConfigs = context.CF_Configurations.ToList();

        List<ConfigurationCategory> configurations = [];
        foreach (var dbConfig in dbConfigs)
        {
            var config = configurations.FirstOrDefault(x => x.Name == dbConfig.Class);
            if (config == null)
            {
                config = new ConfigurationCategory
                {
                    Name = dbConfig.Class,
                    ConfigurationElements = []
                };
                configurations.Add(config);
            }

            config.ConfigurationElements.Add(new ConfigurationElement
            { Value = dbConfig.Value, Name = dbConfig.Property });
        }

        return configurations.ToArray();
    }

    [HasPermission(Permissions.Permissions.Configuration.Edit)]
    public void SetConfiguration(ConfigurationCategory? category)
    {
        if (category == null) return;
        var affectedEntries = context.CF_Configurations.Where(x => x.Class == category.Name);
        foreach (var entry in affectedEntries)
            entry.Value = category.ConfigurationElements.First(x => x.Name == entry.Property).Value;
        context.SaveChanges();
    }

    [HasPermission(Permissions.Permissions.Configuration.Edit)]
    public void SaveConfiguration()
    {
        var jsonString = config.GetConfigAsJson();
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "default-config.json");
        File.WriteAllText(path, jsonString);
    }
}