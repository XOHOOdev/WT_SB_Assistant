﻿namespace WtSbAssistant.Core.Helpers
{
    public static class EnvHelper
    {
        public static Env GetEnvironment()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            ArgumentNullException.ThrowIfNull(environmentName);

            return (Env)Enum.Parse(typeof(Env), environmentName);
        }
    }
}
