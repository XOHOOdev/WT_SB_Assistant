// See https://aka.ms/new-console-template for more information

using WTBattleExtractor.APIs.WtLocal;
using WTBattleExtractor.Parser;

var local = new WtLocal();

while (true)
{
    var res = await local.GetLogsAsync();

    var dmos = DamageMessageParser.ParseMultiple(res);

    if (res is { Damage.Count: > 0 })
        Console.WriteLine(string.Join("\n",
            dmos?.Select(d => $"{d.Time}: {d.Player1} [{d.Vehicle1}] {d.Action} {d.Player2} [{d.Vehicle2}]") ?? []));

    Thread.Sleep(10000);
}