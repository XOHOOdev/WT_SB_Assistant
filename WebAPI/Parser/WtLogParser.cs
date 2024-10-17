﻿using System.Text.RegularExpressions;
using WebAPI.Dto;
using WTBattleExtractor.Dto;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;
using WtSbAssistant.Core.Dto;

namespace WebAPI.Parser;

public static class WtLogParser
{
    private static string[] _killActions = new[] { "shot down", "destroyed" };

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

            var player1String = player1Split.Length > 1 ? string.Join(' ', player1Split.Skip(1)) : "";
            var player2String = player2Split.Length > 1 ? string.Join(' ', player2Split.Skip(1)) : "";

            return new DMO
            {
                Action = action.Value,
                Clan1 = player1Split[0],
                Player1 = player1String,
                Clan2 = player2Split[0],
                Player2 = player2String,
                Vehicle1 = vehicle1,
                Vehicle2 = vehicle2,
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

    public static List<PlayerKd> ParsePlayerKd(List<DMO> dmos)
    {
        List<PlayerKd> kds = [];

        foreach (var dmo in dmos)
        {
            if (kds.FirstOrDefault(kd => kd.Name == dmo.Player1) is not { } firstKd)
            {
                firstKd = new PlayerKd
                {
                    Name = dmo.Player1,
                    VehicleName = dmo.Vehicle1.Substring(1, dmo.Vehicle1.Length - 2)
                };
                kds.Add(firstKd);
            }

            if (string.IsNullOrWhiteSpace(dmo.Player2)) continue;

            if (kds.FirstOrDefault(kd => kd.Name == dmo.Player2) is not { } secondKd)
            {
                secondKd = new PlayerKd
                {
                    Name = dmo.Player2,
                    VehicleName = dmo.Vehicle2.Substring(1, dmo.Vehicle2.Length - 2)
                };
                kds.Add(secondKd);
            }

            if (!_killActions.Contains(dmo.Action)) continue;

            firstKd.Kills++;
            secondKd.Deaths++;
        }

        return kds;
    }
}