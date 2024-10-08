// See https://aka.ms/new-console-template for more information

using WTBattleExtractor.APIs.WtLocal;
using WTBattleExtractor.Dto;
using WtSbAssistant.BlazorUI.Controllers.Dto;

namespace WTBattleExtractor;

class Program
{
    private static bool _sendLogs = false;
    private static readonly WtLocal Local = new WtLocal();
    private static WtLog _log = new WtLog();

    static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

        //Purge log messages
        await Local.GetLogsAsync();

        var watchdog = new Thread(Watch);
        watchdog.Start();

        while (true)
        {
            var res = await Local.GetLogsAsync() ?? new Root();

            var newLogs = res.Damage.Select(d => new WtLogItem { Message = d.Msg, Time = d.Time }).ToList();
            if (res.Damage.Count > 0) Console.WriteLine(string.Join("\n", newLogs.Select(l => l.Message)));

            _log.Logs.AddRange(newLogs);

            if (_sendLogs && _log.Logs.Count > 0)
            {
                await SendLogs();
                _log = new WtLog();
                _sendLogs = false;
            }

            Thread.Sleep(10000);
        }
    }

    private static async void Watch()
    {
        var matchRunning = false;
        while (true)
        {
            var matchFinished = await Local.MatchFinishedAsync();
            _sendLogs = !_sendLogs && matchFinished && matchRunning;
            switch (matchFinished)
            {
                case false when !matchRunning:
                    Console.WriteLine("Match started");
                    break;
                case true when matchRunning:
                    _log.Time = DateTime.Now;
                    Console.WriteLine("Match ended");
                    break;
            }

            matchRunning = !matchFinished;
        }
    }

    private static async Task SendLogs()
    {
        //TODO get win / loss
        //TODO send to WebAPI
        Console.WriteLine("Sending logs");
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        SendLogs().Wait();
    }
}