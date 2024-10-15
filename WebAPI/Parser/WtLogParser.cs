﻿using System.Text.RegularExpressions;
using WTBattleExtractor.Dto;
using WtSbAssistant.Core.Dto;

namespace WebAPI.Parser
{
    public static class WtLogParser
    {
        public static DMO ParseMessage(WtLogItem message)
        {
            try
            {
                var action = new Regex(@"(destroyed)|(severely damaged)|(set afire)|(critically damaged)|(shot down)|(has crashed)|(has achieved)|(has delivered the first strike!)|(has disconnected from the game)|(kd\?NET_PLAYER_DISCONNECT_FROM_GAME)").Match(message.Message).Value;
                var firstPart = new Regex($@".*(?= {Regex.Escape(action)})").Match(message.Message);
                var secondPart = new Regex($@"(?<={Regex.Escape(action)} ).*").Match(message.Message);

                var vehicle2 = string.Empty;
                var player2 = string.Empty;

                if (secondPart.Success)
                {
                    vehicle2 = new Regex(@"(?=\().*(?<=\))$").Match(secondPart.Value).Value;
                    player2 = new Regex($@".*(?= \({vehicle2}\))").Match(secondPart.Value).Value;

                    if (string.IsNullOrEmpty(vehicle2))
                    {
                        action += $" {secondPart.Value}";
                    }
                }

                var vehicle1 = new Regex(@"(?=\().*(?<=\))$").Match(firstPart.Value).Value;
                var player1 = new Regex($@".*(?= \({vehicle1}\))").Match(firstPart.Value).Value;

                var player1Split = player1.Split(' ');
                var player2Split = player2.Split(' ');

                return new DMO
                {
                    Action = action,
                    Clan1 = player1Split[0],
                    Player1 = player1Split.Length > 1 ? player1Split[1] : "",
                    Clan2 = player2Split[0],
                    Player2 = player2Split.Length > 1 ? player2Split[1] : "",
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

            }).ToList();
        }
    }
}
