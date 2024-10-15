// See https://aka.ms/new-console-template for more information

using WTBattleExtractor.APIs.WebServer;
using WTBattleExtractor.APIs.WtLocal;
using WTBattleExtractor.Dto.WtLocal;
using WTBattleExtractor.Dto.WtSbAssistant;

namespace WTBattleExtractor;

internal class Program
{
    private static bool _sendLogs;
    private static readonly WtLocal Local = new();
    private static readonly WtSbServer Server = new();
    private static WtLog _log = new();

    private static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

        var oldLogs = await Local.GetLogsAsync();
        if (!(await Local.MatchFinishedAsync()))
        {
            var lastTime = int.MaxValue;
            oldLogs?.Damage.Reverse();
            foreach (var oldLog in oldLogs?.Damage ?? [])
            {
                if (oldLog.Time <= lastTime)
                {
                    lastTime = oldLog.Time;
                    continue;
                }

                var logsToAdd = oldLogs?.Damage?[..oldLogs.Damage.IndexOf(oldLog)];
                logsToAdd?.Reverse();

                var newLogsToAdd = logsToAdd?.Select(d => new WtLogItem { Message = d.Msg, Time = d.Time }).ToList();
                _log.Logs.AddRange(newLogsToAdd ?? []);
                if (newLogsToAdd?.Count > 0) Console.WriteLine(string.Join("\n", newLogsToAdd.Select(l => l.Message)));
            }
        }

        var watchdog = new Thread(Watch);
        watchdog.Start();

        while (true)
        {
            var res = await Local.GetLogsAsync() ?? new WtLocalRoot();

            var newLogs = res.Damage.Select(d => new WtLogItem { Message = d.Msg, Time = d.Time }).ToList();
            if (newLogs.Count > 0) Console.WriteLine(string.Join("\n", newLogs.Select(l => l.Message)));

            _log.Logs.AddRange(newLogs);

            if (res.Damage.Count > 0 && _log.Time > res.Time)
            {
                _log.Time = res.Time;
            }

            if (!_sendLogs || _log.Logs.Count <= 0) continue;
            await SendLogs();
            _log = new WtLog();
        }
    }

    private static async void Watch()
    {
        var matchRunning = false;
        while (true)
        {
            var matchFinished = await Local.MatchFinishedAsync();
            switch (matchFinished)
            {
                case false when !matchRunning:
                    _sendLogs = false;
                    Console.WriteLine("Match started");
                    break;
                case true when matchRunning:
                    _sendLogs = true;
                    Console.WriteLine("Match ended");
                    break;
            }

            matchRunning = !matchFinished;
        }
    }

    private static async Task SendLogs()
    {
        _log.Result = await Local.GetMatchResult();
        Console.WriteLine("Sending logs");
        await Server.PostLogsAsync(_log);
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        if (_log.Logs.Count <= 0) return;
        SendLogs().Wait();
    }
}