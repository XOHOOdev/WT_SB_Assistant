using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sparta.BlazorUI.Authorization;
using Sparta.Core.DataAccess.DatabaseAccess;
using Sparta.Core.DataAccess.DatabaseAccess.Entities;
using Sparta.Core.Dto.Modules;
using Sparta.Core.Logger;
using Sparta.Modules.Interface;
using Sparta.Modules.Interface.ModuleParameters;
using System.Reflection;
using System.Security.Claims;
using Module = Sparta.Core.DataAccess.DatabaseAccess.Entities.Module;

namespace Sparta.BlazorUI.Data.ModulesData
{
    [HasPermission(Permissions.Permissions.Modules.View)]
    public class ModulesService(ApplicationDbContext<IdentityUser, ApplicationRole, string> context, SpartaLogger logger)
    {
        public IEnumerable<ModuleCategory> GetModuleCategories()
        {
            return Modules.Modules.GetModules().Select(x => new ModuleCategory
            {
                Name = x,
                Modules = context.MD_Modules.Where(m => m.Type.Name == x)
            });
        }

        public ModuleParametersBase? GetModuleParameters(string moduleName)
        {
            var type = typeof(Modules.Modules).Assembly
                .GetTypes()
                .First(t =>
                    t.Namespace != null && t.Namespace.Contains(moduleName) && t.FullName != null &&
                    t.FullName.EndsWith("Parameters"));

            var instance = Activator.CreateInstance(type);

            return instance as ModuleParametersBase;
        }

        public ModuleParametersBase? GetModuleParameters(Module module)
        {
            var type = typeof(Modules.Modules).Assembly
                .GetTypes()
            .First(t =>
                    t.Namespace != null && t.Namespace.Contains(module.Type.Name) && t.FullName != null &&
                    t.FullName.EndsWith("Parameters"));

            if (Activator.CreateInstance(type) is not ModuleParametersBase parameters) return null;

            foreach (var paramInfo in module.Parameters)
            {
                var prop = ((TypeInfo)parameters.GetType()).GetProperty(paramInfo.Name);
                if (prop == null || Activator.CreateInstance(prop.PropertyType) is not ModuleParameterBase property) continue;

                property.Content = paramInfo.Value;

                prop.SetValue(parameters, property);
            }

            return parameters;
        }

        public void CreateModule(List<ParamInfo>? parameters, ModuleCategory category)
        {
            if (parameters == null || !CheckParameters(parameters)) return;
            var moduleType = context.MD_ModuleType.FirstOrDefault(t => t.Name == category.Name) ?? context.Add(new ModuleType { Name = category.Name }).Entity;

            context.Add(new Module
            {
                Name = "New Module",
                Enabled = false,
                Parameters = parameters.Select(p => new ModuleParameter
                {
                    Name = p.Name,
                    Value = p.Content
                }).ToList(),
                Type = moduleType
            });

            SaveChanges();
        }

        public void SetModuleParameters(Module module, List<ParamInfo>? parameters)
        {
            if (parameters == null || !CheckParameters(parameters)) return;
            module.Parameters.ForEach(p => p.Value = parameters.FirstOrDefault(x => x.Name == p.Name)?.Content ?? p.Value);

            SaveChanges();
        }

        public void SetOptions(ref List<ParamInfo> parameters, ClaimsPrincipal user)
        {
            if (parameters.Any(p => (int)p.Type >= (int)ParameterType.DiscordChannel)) parameters.Insert(0, new ParamInfo { Name = "ModuleParameterGuild", Type = ParameterType.DiscordGuild });
            if (user.Identity is not { } identity) return;
            if (context.Users.FirstOrDefault(x => x.UserName == identity.Name) is not { } identityUser) return;

            var userRoles = context.UserRoles.Where(r => r.UserId == identityUser.Id).Select(r => r.RoleId).ToArray();

            var availableGuilds = context.DC_Guilds.Where(g => g.ApplicationRoles.Any(r => userRoles.Contains(r.Id))).ToArray();

            foreach (var parameter in parameters)
            {
                parameter.Options = parameter.Type switch
                {
                    ParameterType.Text or ParameterType.LargeText or ParameterType.Number or ParameterType.Bool => null,
                    ParameterType.HllServer => context.SV_Servers.ToArray(),
                    ParameterType.DiscordChannel => context.DC_Channels.Where(c => availableGuilds.Contains(c.DiscordGuild)).ToArray(),
                    ParameterType.DiscordUser => context.DC_Users.Where(u => u.DiscordGuilds.Any(g => availableGuilds.Contains(g))).ToArray(),
                    ParameterType.DiscordRole => context.DC_Roles.Where(r => availableGuilds.Contains(r.Guild)).ToArray(),
                    ParameterType.DiscordGuild => availableGuilds,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public void DeleteModule(Module module)
        {
            context.Remove(module);

            SaveChanges();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        private bool CheckParameters(List<ParamInfo> parameters)
        {
            try
            {
                return parameters.All(parameter => parameter.Options.IsNullOrEmpty() ||
                    parameter.Options.Select(o => o.Id.ToString()).Contains(parameter.Content));
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return false;
            }
        }
    }
}
