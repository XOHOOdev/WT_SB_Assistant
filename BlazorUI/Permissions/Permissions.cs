namespace Sparta.BlazorUI.Permissions;

public class Permissions
{
    public class UserManagement
    {
        public const string View = "UserManagement.View";
        public const string Edit = "UserManagement.Edit";
        public const string Delete = "UserManagement.Delete";
        public const string Create = "UserManagement.Create";
    }

    public class Configuration
    {
        public const string View = "Configuration.View";
        public const string Edit = "Configuration.Edit";
    }

    public class Logging
    {
        public const string View = "Logging.View";
    }

    public class Modules
    {
        public const string View = "Modules.View";
        public const string Edit = "Modules.Edit";
        public const string Delete = "Modules.Delete";
        public const string Create = "Modules.Create";
    }

    public class Servers
    {
        public const string View = "Servers.View";
        public const string Edit = "Servers.Edit";
        public const string Delete = "Servers.Delete";
        public const string Create = "Servers.Create";
    }
}