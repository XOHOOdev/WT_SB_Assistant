namespace Sparta.BlazorUI.Data.ConfigurationData;

public class ConfigurationCategory
{
    public string Name { get; set; } = null!;
    public List<ConfigurationElement> ConfigurationElements { get; set; } = null!;
}