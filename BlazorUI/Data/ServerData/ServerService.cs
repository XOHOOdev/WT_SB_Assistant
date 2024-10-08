using Microsoft.AspNetCore.Identity;
using Sparta.BlazorUI.Authorization;
using Sparta.Core.DataAccess.DatabaseAccess;
using Sparta.Core.DataAccess.DatabaseAccess.Entities;

namespace Sparta.BlazorUI.Data.ServerData
{
    [HasPermission(Permissions.Permissions.Servers.View)]
    public class ServerService(ApplicationDbContext<IdentityUser, ApplicationRole, string> context)
    {
        public IEnumerable<Server> GetServers()
        {
            return context.SV_Servers;
        }

        public void CreateServer(string name)
        {
            context.Add(new Server
            {
                Name = name,
                Password = string.Empty,
                Username = string.Empty,
                Url = string.Empty
            });
            SaveChanges();
        }

        public void DeleteServer(Server server)
        {
            context.Remove(server);
            SaveChanges();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
