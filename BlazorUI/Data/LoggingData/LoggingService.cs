using Microsoft.AspNetCore.Identity;
using WtSbAssistant.Core.DataAccess.DatabaseAccess;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;
using WtSbAssistant.Core.Logger;

namespace WtSbAssistant.BlazorUI.Data.LoggingData
{
    public class LoggingService(ApplicationDbContext<IdentityUser, ApplicationRole, string> context)
    {
        public IEnumerable<LogMessage> GetLogs(int count, DateTimeOffset minTimeOffset, DateTimeOffset maxTimeOffset, LogSeverity severity)
        {
            var logs = context.LG_LogMessages
                 .Where(l =>
                     (int)l.Severity <= (int)severity
                     && (maxTimeOffset == DateTimeOffset.MinValue || l.Time <= maxTimeOffset)
                     && (minTimeOffset == DateTimeOffset.MinValue || l.Time >= minTimeOffset)).ToArray();

            var logsOrdered = logs.OrderByDescending(l => l.Time).Take(count);

            return logsOrdered;
        }
    }
}
