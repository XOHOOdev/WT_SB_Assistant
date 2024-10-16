using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dto;
using WTBattleExtractor.Dto;
using WtSbAssistant.Core.DataAccess.DatabaseAccess;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;
using WtSbAssistant.Core.Helpers;
using WtSbAssistant.Core.Logger;

namespace WebAPI.DataAccess
{
    public class DatabaseManager(WtSbAssistantLogger logger, ApplicationDbContext<IdentityUser, ApplicationRole, string> context)
    {
        public async Task InsertDataAsync(List<DMO> dmos, DateTime starTime)
        {
            var basicInsertResult = await InsertBasicDataAsync(dmos); //TODO return failure code if more than 2 clans / more than 16 players (not CW)

            await ConnectPlayerClanAsync(dmos, starTime);

            var match = await CreateMatchAsync(dmos, starTime);

            if (match == null) return; //TODO return failure code if match already exists

            await CreateVehiclesAsync(dmos);

            await ConnectPlayerClanMatchAsync(match, basicInsertResult.Item1, basicInsertResult.Item2);
        }

        public async Task<int> UpdateVehiclesAsync(List<ApiVehicle> vehicles)
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
                return 0;
            }

            var dbVehicles = vehicles.Select(v => new WtVehicle
            {
                Name = v.Identifier,
                Nation = nations.First(n => n.Name == v.Country),
                BattleRating = battleRatings.First(b => b.BattleRating == (decimal)v.RealisticGroundBr)
            }).ToList();

            context.WT_Vehicles.AddRange(dbVehicles);

            context.WT_VehicleVehicleTypes.AddRange(vehicles
                .Select(v => v.VehicleSubTypes
                    .Concat(new[] { v.VehicleType })
                    .Select(
                        vt => new WtVehicleVehicleType
                        {
                            Vehicle = dbVehicles.First(dv => dv.Name == v.Identifier),
                            VehicleType = vehicleTypes.First(t => t.Name == vt)
                        })
                )
                .SelectMany(vvt => vvt)
                .ToList());

            try
            {
                await context.SaveChangesAsync();
                return vehicles.Count;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return 0;
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
                logger.LogDebug($"Found players: {string.Join(", ", playerList.Select(p => p.Name))} in Clans {string.Join(", ", clanList.Select(c => c.Name))}");
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

        private async Task<WtMatch?> CreateMatchAsync(List<DMO> dmos, DateTime starTime)
        {
            var newMatch = new WtMatch
            {
                BattleRatingId = context.WT_BattleRatings.First(br => br.From <= starTime && br.Until >= starTime).UniqueId,
                MatchStart = starTime,
                MatchEnd = dmos.Last().Time,
            };

            var matches = context.WT_Matches.ToList();
            if (matches.Contains(newMatch))
            {
                return null;
            }

            context.Add(newMatch);

            try
            {
                await context.SaveChangesAsync();
                return newMatch;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return null;
            }
        }

        private async Task CreateVehiclesAsync(List<DMO> dmos)
        {
            var vehicles = dmos.SelectMany(d => new[] { d.Vehicle1, d.Vehicle2 })
                .Distinct()
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => new Vehicle { RawName = v })
                .ToList();
        }

        private async Task<bool> ConnectPlayerClanMatchAsync(WtMatch match, List<WtPlayer> players, List<WtClan> clans)
        {
            context.WT_ClanMatch.BulkInsert(
                clans.Select(c => new WtClanMatch
                {
                    ClanId = c.UniqueId,
                    MatchId = match.UniqueId
                }).ToList());

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return false;
            }
        }
    }
}
