using Sparta.Core.DataAccess.DatabaseAccess.Entities;

namespace Sparta.BlazorUI.Data.ModulesData
{
    public class ModuleCategory
    {
        public string Name { get; set; } = null!;

        public IEnumerable<Module> Modules { get; set; } = [];
    }
}
