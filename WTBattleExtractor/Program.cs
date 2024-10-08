// See https://aka.ms/new-console-template for more information

using WTBattleExtractor.APIs.WtLocal;
using WtSbAssistant.BlazorUI.Controllers.Dto;

var local = new WtLocal();

while (true)
{
    var res = await local.GetLogsAsync();
    if (res == null) continue;
    var log = new WtLog
    {
        Logs = res.Damage.Select(d => new WtLogItem { Message = d.Msg, Time = d.Time }).ToList(),
        Time = res.Received
    };

    Thread.Sleep(10000);
}