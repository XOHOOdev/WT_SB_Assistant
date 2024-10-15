namespace WtSbAssistant.BlazorUI.Permissions;

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

    public class WtDataManagement
    {
        public const string View = "WtDataManagement.View";
        public const string Create = "WtDataManagement.Create";
    }
}