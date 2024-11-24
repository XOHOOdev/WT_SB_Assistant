using System.Text.RegularExpressions;
using WTBattleExtractor.Dto;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;
using WtSbAssistant.Core.Dto;
using Exception = System.Exception;

namespace WebAPI.Parser;

public static class WtLogParser
{
    private static readonly string[] KillActions = ["shot down", "destroyed"];

    public static DMO ParseMessage(WtLogItem message)
    {
        try
        {
            var action =
                new Regex(
                        @"(destroyed)|(severely damaged)|(set afire)|(critically damaged)|(shot down)|(has crashed)|(has achieved.*)|(has delivered the first strike!)|(has delivered the final blow!)")
                    .Match(message.Message);

            if (!action.Success) return new DMO();

            var firstPart = new Regex($@".*(?= {Regex.Escape(action.Value)})").Match(message.Message);
            var secondPart = new Regex($@"(?<={Regex.Escape(action.Value)} ).*").Match(message.Message);

            var vehicle2 = string.Empty;
            var player2 = string.Empty;

            if (secondPart.Success)
            {
                vehicle2 = new Regex(@"(?=\().*(?<=\))$").Match(secondPart.Value).Value;
                player2 = new Regex($@".*(?= {Regex.Escape(vehicle2)})").Match(secondPart.Value).Value;
            }

            var vehicle1 = new Regex(@"(?=\().*(?<=\))$").Match(firstPart.Value).Value;
            var player1 = new Regex($@".*(?= {Regex.Escape(vehicle1)})").Match(firstPart.Value).Value;

            var player1Split = player1.Split(' ');
            var player2Split = player2.Split(' ');

            var player1String = player1Split.Length > 1 ? string.Join(' ', player1Split.Skip(1)) : player1Split[0];
            var player2String = player2Split.Length > 1 ? string.Join(' ', player2Split.Skip(1)) : player2Split[0];

            return new DMO
            {
                Action = action.Value,
                Clan1 = player1Split.Length > 1 ? player1Split[0] : "",
                Player1 = player1String,
                Clan2 = player2Split.Length > 1 ? player2Split[0] : "",
                Player2 = player2String,
                Vehicle1 = vehicle1.Substring(1, vehicle1.Length - 2),
                Vehicle2 = string.IsNullOrWhiteSpace(vehicle2) ? "" : vehicle2.Substring(1, vehicle2.Length - 2),
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new DMO();
        }

    }

    public static List<DMO> ParseLog(WtLog log)
    {
        return log.Logs.Select(m => new { DMO = ParseMessage(m), m }).Select(d =>
        {
            d.DMO.Time = log.Time.AddSeconds(d.m.Time);
            return d.DMO;

        }).Where(d => !string.IsNullOrWhiteSpace(d.Action)).ToList();
    }

    public static WtMatchResult ParseMatchResult(string result)
    {
        return result switch
        {
            "fail" => WtMatchResult.Loss,
            "success" => WtMatchResult.Win,
            _ => WtMatchResult.Unknown
        };
    }

    public static List<WtBattleAction> ParsePlayerAction(List<DMO> dmos, List<WtPlayer> player, WtMatch match)
    {
        List<WtBattleAction> actions = [];

        foreach (var dmo in dmos)
        {
            WtBattleAction? firstAction;
            WtBattleAction? secondAction;

            try
            {
                switch (dmo.Action)
                {
                    case "destroyed":
                    case "shot down":
                        firstAction = new WtBattleAction
                        {
                            ActionType = WtBattleActionType.Killed,
                            Player = player.First(p => p.Name == dmo.Player1),
                            Match = match,
                            Vehicle = new WtVehicle
                            {
                                Name = dmo.Vehicle1
                            }
                        };
                        secondAction = new WtBattleAction
                        {
                            ActionType = WtBattleActionType.Died,
                            Player = player.First(p => p.Name == dmo.Player2),
                            Match = match,
                            Vehicle = new WtVehicle
                            {
                                Name = dmo.Vehicle2
                            }
                        };
                        firstAction.LinkedAction = secondAction;
                        actions.Add(firstAction);
                        break;
                    case "has crashed":
                        actions.Add(new WtBattleAction
                        {
                            ActionType = WtBattleActionType.Died,
                            Player = player.First(p => p.Name == dmo.Player1),
                            Match = match,
                            Vehicle = new WtVehicle
                            {
                                Name = dmo.Vehicle1
                            }
                        });
                        break;
                    case "severely damaged":
                    case "set afire":
                    case "critically damaged":
                        firstAction = new WtBattleAction
                        {
                            ActionType = WtBattleActionType.Damaged,
                            Player = player.First(p => p.Name == dmo.Player1),
                            Match = match,
                            Vehicle = new WtVehicle
                            {
                                Name = dmo.Vehicle1
                            }
                        };
                        secondAction = new WtBattleAction
                        {
                            ActionType = WtBattleActionType.GotDamaged,
                            Player = player.First(p => p.Name == dmo.Player2),
                            Match = match,
                            Vehicle = new WtVehicle
                            {
                                Name = dmo.Vehicle2
                            }
                        };
                        firstAction.LinkedAction = secondAction;
                        actions.Add(firstAction);
                        break;
                    case { } act when act.Contains("has achieved"):
                        actions.Add(new WtBattleAction
                        {
                            ActionType = WtBattleActionType.Achieved,
                            Player = player.First(p => p.Name == dmo.Player1),
                            Match = match,
                            Vehicle = new WtVehicle
                            {
                                Name = dmo.Vehicle1
                            }
                        });
                        break;
                    default: continue;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return actions;
    }
}