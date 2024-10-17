using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class DatabaseManager(WtSbAssistantLogger logger, ApplicationDbContext<IdentityUser, ApplicationRole, string> context)
    {
        public async Task InsertDataAsync(WtLog log)
        {
            var dmos = WtLogParser.ParseLog(log);

            var basicInsertResult = await InsertBasicDataAsync(dmos); //TODO return failure code if more than 2 clans / more than 16 players (not CW)

            await ConnectPlayerClanAsync(dmos, log.Time);

            var matchResult = await CreateMatchAsync(dmos, log);

            if (!matchResult.Success) return; //TODO return failure code if match already exists

            var linkResult = await ConnectPlayerClanMatchAsync(matchResult.Value ?? new WtMatch(), basicInsertResult.Item1, basicInsertResult.Item2, dmos);
        }

        public async Task<Result<int>> UpdateVehiclesAsync(List<ApiVehicle> vehicles, List<VehicleIdentifier> identifiers)
        {
            await context.WT_Vehicles.ExecuteDeleteAsync();
            await context.WT_VehicleVehicleTypes.ExecuteDeleteAsync();

            var battleRatings = context.WT_VehicleBattleRatings
                .BulkInsert(
                    vehicles
                        .Select(v => new WtVehicleBattleRating()
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

        private async Task<Tuple<List<WtPlayer>, List<WtClan>>> InsertBasicDataAsync(List<DMO> dmos)
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

            try
            {
                await context.SaveChangesAsync();
                return new Tuple<List<WtPlayer>, List<WtClan>>(playerList, clanList);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new Tuple<List<WtPlayer>, List<WtClan>>([], []);
            }
        }

        private async Task ConnectPlayerClanAsync(List<DMO> dmos, DateTime starTime)
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
            playerClanList.ForEach(cp => cp.LastSeen = starTime);

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

        private async Task<Result<bool>> ConnectPlayerClanMatchAsync(WtMatch match, List<WtPlayer> players, List<WtClan> clans, List<DMO> dmos)
        {
            context.WT_ClanMatch.BulkInsert(
                clans.Select(c => new WtClanMatch
                {
                    ClanId = c.UniqueId,
                    MatchId = match.UniqueId
                }).ToList());

            var kds = WtLogParser.ParsePlayerKd(dmos);
            var vehicles = await FindBestVehicleMatch(kds, match);

            context.WT_VehiclePlayerMatches.AddRange(kds.Select(k => new WtVehiclePlayerMatch
            {
                Player = players.First(p => p.Name == k.Name),
                Match = match,
                Kills = k.Kills,
                Deaths = k.Deaths,
                Vehicle = vehicles.First(v => v.PlayerName == k.Name).BestMatch
            }));

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

        private async Task<List<PlayerVehicle>> FindBestVehicleMatch(List<PlayerKd> kds, WtMatch match)
        {
            var playerVehicles = kds.Select(k => new PlayerVehicle
            {
                PlayerName = k.Name,
                VehicleName = k.VehicleName,
                PossibleMatches = context.WT_Vehicles.Where(v => v.Name.Equals(k.VehicleName) && v.BattleRating.BattleRating <= match.BattleRating.BattleRating).ToList()
            }).ToList();

            foreach (var playerVehicle in playerVehicles)
            {
                if (playerVehicle.PossibleMatches.Count == 1)
                {
                    playerVehicle.BestMatch = playerVehicle.PossibleMatches.First();
                    continue;
                }

                playerVehicle.PossibleMatches = context.WT_Vehicles
                    .Where(v => v.Name.Contains(playerVehicle.VehicleName) &&
                                v.BattleRating.BattleRating <= match.BattleRating.BattleRating)
                    .Include(wtVehicle => wtVehicle.BattleRating).ToList();

                playerVehicle.BestMatch = playerVehicle.PossibleMatches.OrderBy(v => v.BattleRating.BattleRating).First();
            }

            return playerVehicles;
        }
    }
}
