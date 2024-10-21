using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WtSbAssistant.BlazorUI.Data.WtDataManagementData.Dto;
using WtSbAssistant.Core.DataAccess.DatabaseAccess;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;
using WtSbAssistant.Core.Helpers;
using WtSbAssistant.Core.Logger;

namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData
{
    public class ClanDataManagement(ApplicationDbContext<IdentityUser, ApplicationRole, string> context, ConfigHelper config, WtSbAssistantLogger logger)
    {
        private readonly string[] _airTypes =
        [
            "aa_fighter", "assault", "attack_helicopter", "bomber", "dive_bomber", "fighter", "frontline_bomber",
            "hydroplane", "interceptor", "jet_bomber", "jet_fighter", "light_bomber", "longrange_bomber",
            "naval_aircraft", "strike_aircraft", "strike_ucav", "torpedo", "utility_helicopter"
        ];

        public List<string> GetClanNames()
        {
            var ownClan = config.GetConfig("WtData", "OwnClan")?.ToLower() ?? "";

            return context.WT_Clans.Where(c => !c.Name.ToLower().Contains(ownClan)).Select(c => c.Name).AsEnumerable().OrderBy(n => Regex.Replace(n, "[^a-zA-Z0-9]", "")).ToList();
        }

        public List<MatchClanModel> GetClans(string? clanName = null, int countReturns = 10)
        {
            try
            {
                var ownClan = config.GetConfig("WtData", "OwnClan")?.ToLower() ?? "";

                var matches = context.WT_Matches
                    .Where(m => clanName == null ||
                                m.WtClanMatches.Any(cm => cm.Clan.Name.ToLower().Contains(clanName.ToLower())))
                    .OrderByDescending(m => m.MatchStart).Take(countReturns).Select(m =>
                        new MatchClanModel
                        {
                            Id = m.WtClanMatches.First(cm => !cm.Clan.Name.ToLower().Contains(ownClan)).ClanId,
                            Name = m.WtClanMatches.First(cm => !cm.Clan.Name.ToLower().Contains(ownClan)).Clan.Name,
                            LastMatch = new MatchModel
                            {
                                Id = m.UniqueId,
                                StartTime = DateTime.SpecifyKind(m.MatchStart, DateTimeKind.Utc),
                                Duration = m.MatchEnd.Subtract(m.MatchStart),
                                Result = m.Result,
                                Players = m.WtBattleActions.Where(ba =>
                                        !ba.Player.WtClanPlayers.OrderByDescending(cp => cp.LastSeen).First().Clan.Name
                                            .ToLower().Contains(ownClan))
                                    .GroupBy(ba => ba.PlayerId).Select(
                                        bag => new MatchPlayerModel
                                        {
                                            Id = bag.Key,
                                            Name = bag.ToList()[0].Player.Name,
                                            Vehicle = new VehicleModel
                                            {
                                                Name = bag.First().Vehicle.Name,
                                                AirKills = bag.Count(a =>
                                                    a.ActionType == WtBattleActionType.Killed &&
                                                    a.LinkedAction != null &&
                                                    a.LinkedAction.Vehicle.VehicleTypes.Any(v =>
                                                        _airTypes.Contains(v.VehicleType.Name))),
                                                GroundKills = bag.Count(a =>
                                                    a.ActionType == WtBattleActionType.Killed &&
                                                    a.LinkedAction != null &&
                                                    !a.LinkedAction.Vehicle.VehicleTypes.Any(v =>
                                                        _airTypes.Contains(v.VehicleType.Name))),
                                                Deaths = bag.Count(a => a.ActionType == WtBattleActionType.Died)
                                            }

                                        }).ToList()
                            }
                        }).ToList();
                return matches;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return [];
            }
        }

        public ClanModel GetClan(int clanId)
        {
            try
            {
                var clan = context.WT_Clans.Include(wtClan => wtClan.WtClanMatch)
                    .ThenInclude(wtClanMatch => wtClanMatch.Match).ThenInclude(wtMatch => wtMatch.WtBattleActions)
                    .Include(wtClan => wtClan.WtClanPlayers).First(c => c.UniqueId == clanId);

                return new ClanModel
                {
                    Id = clan.UniqueId,
                    Name = clan.Name,
                    PlayerModels = clan.WtClanMatch.Select(cm => cm.Match).SelectMany(m => m.WtBattleActions)
                        .Where(ba => clan.WtClanPlayers.Select(cp => cp.PlayerId).Contains(ba.PlayerId))
                        .GroupBy(ba => ba.PlayerId).Select(ga => new ClanPlayerModel
                        {
                            Id = ga.First().PlayerId,
                            Name = ga.First().Player.Name,
                            Vehicles = ga.Select(ba => ba.Vehicle).GroupBy(v => v.UniqueId).Select(gv =>
                                new VehicleModel
                                {
                                    Id = gv.First().UniqueId,
                                    Name = gv.First().Name,
                                    Matches = ga.Where(ba => ba.VehicleId == gv.First().UniqueId)
                                        .Select(ba => ba.MatchId).Distinct().Count(),
                                    AirKills = ga.Count(ba =>
                                        ba.VehicleId == gv.First().UniqueId &&
                                        ba.ActionType == WtBattleActionType.Killed &&
                                        (ba.LinkedAction?.Vehicle.VehicleTypes.Any(v =>
                                            _airTypes.Contains(v.VehicleType.Name)) ?? false)),
                                    GroundKills = ga.Count(ba =>
                                        ba.VehicleId == gv.First().UniqueId &&
                                        ba.ActionType == WtBattleActionType.Killed &&
                                        (!ba.LinkedAction?.Vehicle.VehicleTypes.Any(v =>
                                            _airTypes.Contains(v.VehicleType.Name)) ?? false)),
                                    Deaths = ga.Count(ba =>
                                        ba.VehicleId == gv.First().UniqueId && ba.ActionType == WtBattleActionType.Died)
                                }).ToList()
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new ClanModel();
            }
        }
    }
}
