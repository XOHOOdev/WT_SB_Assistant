using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebAPI.Dto;
using WebAPI.Parser;
using WTBattleExtractor.Dto;
using WtSbAssistant.Core.DataAccess.DatabaseAccess;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;
using WtSbAssistant.Core.Dto;
using WtSbAssistant.Core.Helpers;
using WtSbAssistant.Core.Logger;

namespace WebAPI.DataAccess
{
    public class DatabaseManager(WtSbAssistantLogger logger, ApplicationDbContext<IdentityUser, ApplicationRole, string> context, VehicleDataManager vehicleData, ConfigHelper config)
    {
        public async Task<Result<bool>> InsertDataAsync(WtLog log)
        {
            var dmos = WtLogParser.ParseLog(log);

            var basicInsertResult = await InsertBasicDataAsync(dmos);
            if (!basicInsertResult.Success) return new Result<bool>(success: false, message: basicInsertResult.Message, exception: basicInsertResult.Exception);

            await ConnectPlayerClanAsync(dmos, log.Time);

            var matchResult = await CreateMatchAsync(dmos, log);
            if (!matchResult.Success) return new Result<bool>(success: false, message: matchResult.Message, exception: matchResult.Exception);

            var linkResult = await CreateBattleActions(matchResult.Value ?? new WtMatch(), basicInsertResult.Value?.Item1 ?? [], basicInsertResult.Value?.Item2 ?? [], dmos);

            if (matchResult.Value?.Result == WtMatchResult.Unknown) await PredictMatchOutcomeAsync(matchResult.Value.UniqueId);

            return new Result<bool>(linkResult.Success, linkResult.Success, linkResult.Exception, linkResult.Message);
        }

        public async Task<Result<int>> UpdateVehiclesAsync(List<ApiVehicle> vehicles, List<VehicleIdentifier> identifiers)
        {
            await context.WT_Vehicles.ExecuteDeleteAsync();
            await context.WT_VehicleVehicleTypes.ExecuteDeleteAsync();

            var battleRatings = context.WT_VehicleBattleRatings
                .BulkInsert(
                    vehicles
                        .Select(v => new WtVehicleBattleRating
                        {
                            BattleRating = (decimal)v.RealisticGroundBr
                        })
                        .Distinct()
                        .ToList()
                    );

            var vehicleTypes = context.WT_VehicleTypes
                .BulkInsert(
                    vehicles
                        .SelectMany(v => v.VehicleSubTypes.Concat(new[] { v.VehicleType }))
                        .Distinct()
                        .Select(vt =>
                            new WtVehicleType
                            {
                                Name = vt
                            })
                        .ToList()
                    );

            var nations = context.WT_Nations
                .BulkInsert(
                    vehicles.Select(v => new WtNation
                    {
                        Name = v.Country
                    })
                    .Distinct()
                    .ToList());

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Result<int>(exception: ex);
            }

            var dbVehicles = vehicles.Select(v => new WtVehicle
            {
                Identifier = v.Identifier,
                Nation = nations.First(n => n.Name == v.Country),
                BattleRating = battleRatings.First(b => b.BattleRating == (decimal)v.RealisticGroundBr),
                Name = identifiers.FirstOrDefault(i => i.Id == v.Identifier)?.Name ?? ""
            }).ToList();

            context.WT_Vehicles.AddRange(dbVehicles);

            try
            {
                context.WT_VehicleVehicleTypes.AddRange(vehicles
                    .Select(v => v.VehicleSubTypes
                        .Concat(new[] { v.VehicleType })
                        .Select(
                            vt => new WtVehicleVehicleType
                            {
                                Vehicle = dbVehicles.First(dv => dv.Identifier == v.Identifier),
                                VehicleType = vehicleTypes.First(t => t.Name == vt)
                            })
                    )
                    .SelectMany(vvt => vvt)
                    .ToList());
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Result<int>(exception: ex);
            }

            try
            {
                await context.SaveChangesAsync();
                return new Result<int>(dbVehicles.Count);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Result<int>(exception: ex);
            }
        }

        private async Task<Result<Tuple<List<WtPlayer>, List<WtClan>>>> InsertBasicDataAsync(List<DMO> dmos)
        {
            var clanList = context.WT_Clans
                .BulkInsert(
                    dmos
                        .SelectMany(d => new[] { d.Clan1, d.Clan2 })
                        .Distinct()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => new WtClan { Name = s })
                        .ToList()
                );

            var playerList = context.WT_Players
                .BulkInsert(
                    dmos
                        .SelectMany(d => new[] { d.Player1, d.Player2 })
                        .Distinct()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => new WtPlayer { Name = s })
                        .ToList()
                );
            if (clanList.Count > 16)
                return new Result<Tuple<List<WtPlayer>, List<WtClan>>>(success: false, message: "Match is not CW");

            try
            {
                await context.SaveChangesAsync();
                return new Result<Tuple<List<WtPlayer>, List<WtClan>>>(new Tuple<List<WtPlayer>, List<WtClan>>(playerList, clanList));
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Result<Tuple<List<WtPlayer>, List<WtClan>>>(exception: ex);
            }
        }

        private async Task ConnectPlayerClanAsync(List<DMO> dmos, DateTime startTime)
        {
            var playerClanList = context.WT_ClanPlayers
                .BulkInsert(
                    dmos
                        .SelectMany(d => new[] { new { Player = d.Player1, Clan = d.Clan1 }, new { Player = d.Player2, Clan = d.Clan2 } })
                        .Distinct()
                        .Where(cp => !string.IsNullOrWhiteSpace(cp.Clan) && !string.IsNullOrWhiteSpace(cp.Player))
                        .Select(cp => new WtClanPlayer
                        {
                            ClanId = context.WT_Clans.First(c => c.Name == cp.Clan).UniqueId,
                            PlayerId = context.WT_Players.First(p => p.Name == cp.Player).UniqueId,
                        })
                        .ToList()
                );
            playerClanList.ForEach(cp => cp.LastSeen = startTime);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private async Task<Result<WtMatch>> CreateMatchAsync(List<DMO> dmos, WtLog log)
        {
            try
            {
                var newMatch = new WtMatch
                {
                    BattleRatingId = context.WT_BattleRatings.First(br => br.From <= log.Time && br.Until >= log.Time).UniqueId,
                    MatchStart = log.Time,
                    MatchEnd = dmos.Last().Time,
                    Result = WtLogParser.ParseMatchResult(log.Result ?? "")
                };

                var matches = context.WT_Matches.ToList();
                if (matches.Contains(newMatch))
                {
                    return new Result<WtMatch>(message: "Match already exists");
                }

                context.Add(newMatch);

                await context.SaveChangesAsync();
                return new Result<WtMatch>(value: newMatch);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Result<WtMatch>(exception: ex);
            }
        }

        private async Task<Result<bool>> CreateBattleActions(WtMatch match, List<WtPlayer> players, List<WtClan> clans, List<DMO> dmos)
        {
            context.WT_ClanMatch.BulkInsert(
                clans.Select(c => new WtClanMatch
                {
                    ClanId = c.UniqueId,
                    MatchId = match.UniqueId
                }).ToList());

            var actions = WtLogParser.ParsePlayerAction(dmos, players, match);
            await FindBestVehicleMatchAsync(actions, match);

            context.WT_BattleAction.AddRange(actions);

            try
            {
                await context.SaveChangesAsync();
                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Result<bool>(exception: ex);
            }
        }

        private async Task FindBestVehicleMatchAsync(List<WtBattleAction> actions, WtMatch match)
        {
            var allActions = actions.SelectMany(a => new[] { a, a.LinkedAction }).Where(a => a != null);
            var playerVehicles = allActions.Select(a => new PlayerVehicle
            {
                BattleAction = a,
                VehicleName = a.Vehicle.Name,
                PossibleMatches = context.WT_Vehicles.Where(v => v.Name.Equals(a.Vehicle.Name) && v.BattleRating.BattleRating <= match.BattleRating.BattleRating).ToList()
            }).ToList();


            foreach (var playerVehicle in playerVehicles)
            {
                if (playerVehicle.PossibleMatches.Count == 1)
                {
                    playerVehicle.BattleAction.Vehicle = playerVehicle.PossibleMatches.First();
                    continue;
                }

                playerVehicle.PossibleMatches = context.WT_Vehicles
                    .Where(v => v.Name.Contains(playerVehicle.VehicleName) &&
                                v.BattleRating.BattleRating <= match.BattleRating.BattleRating)
                    .Include(wtVehicle => wtVehicle.BattleRating).ToList();

                try
                {
                    if (playerVehicle.PossibleMatches.MaxBy(v => v.BattleRating.BattleRating) is not { } findVehicle)
                    {
                        findVehicle = await FindVehicle(playerVehicle.VehicleName);
                    }

                    playerVehicle.BattleAction.Vehicle = findVehicle;
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                }
            }
        }

        private async Task<WtVehicle> FindVehicle(string name)
        {
            const string regex = @"\s|_";

            if (context.WT_Vehicles.AsEnumerable().FirstOrDefault(v => Regex.Replace(v.Identifier.ToLowerInvariant(), regex, "").Contains(Regex.Replace(name.ToLowerInvariant(), regex, ""))) is not { } vehicle)
            {
                var scrapeVehicle = vehicleData.ScrapeVehicle(name);
                vehicle = new WtVehicle
                {
                    Identifier = name,
                    Nation = context.WT_Nations.First(n => n.Name == scrapeVehicle.Country),
                    BattleRating = context.WT_VehicleBattleRatings.First(b => b.BattleRating == (decimal)scrapeVehicle.RealisticGroundBr),
                };

                context.WT_Vehicles.Add(vehicle);
                context.WT_VehicleVehicleTypes.Add(new WtVehicleVehicleType
                {
                    VehicleType = context.WT_VehicleTypes.First(vt => vt.Name == scrapeVehicle.VehicleType),
                    Vehicle = vehicle
                });
            }
            vehicle.Name = name;
            await context.SaveChangesAsync();

            return vehicle;
        }

        private async Task PredictMatchOutcomeAsync(int matchId)
        {
            try
            {
                var ownClanName = config.GetConfig("WtData", "OwnClan")?.ToLower() ?? "";

                var match = context.WT_Matches.Include(wtMatch => wtMatch.WtClanMatches)
                    .ThenInclude(wtClanMatch => wtClanMatch.Clan).Include(wtMatch => wtMatch.WtBattleActions)
                    .ThenInclude(wtBattleAction => wtBattleAction.Player)
                    .ThenInclude(wtPlayer => wtPlayer.WtClanPlayers)
                    .ThenInclude(wtClanPlayer => wtClanPlayer.Clan).First(m => m.UniqueId == matchId);

                var clanActions = match.WtBattleActions
                    .GroupBy(ba => ba.Player.WtClanPlayers.First(cp => cp.LastSeen == match.MatchStart).Clan).ToList();

                var clanKills = clanActions.ToDictionary(ga => ga.Key,
                    ga => ga.Count(ba => ba.ActionType == WtBattleActionType.Died));

                var ownClanDeaths = clanKills.First(kvp => kvp.Key.Name.ToLower().Contains(ownClanName));
                var enemyClanDeaths = clanKills.First(ca => !ca.Key.Equals(ownClanDeaths.Key));

                match.Result = (ownClanDeaths.Value - enemyClanDeaths.Value) switch
                {
                    < 0 => WtMatchResult.ProbWin,
                    > 0 => WtMatchResult.ProbLoss,
                    _ => match.Result
                };

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
    }
}
