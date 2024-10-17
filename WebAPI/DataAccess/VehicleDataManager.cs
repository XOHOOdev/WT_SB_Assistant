using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections;
using WebAPI.Dto;
using WtSbAssistant.Core.Helpers;
using WtSbAssistant.Core.Logger;

namespace WebAPI.DataAccess
{
    public class VehicleDataManager(ConfigHelper config, WtSbAssistantLogger logger)
    {
        private readonly HttpClient _apiClient = new HttpClient
        {
            BaseAddress = new Uri(config.GetConfig("VehicleApi", "BaseUrl") ?? "")
        };

        private readonly HtmlWeb _web = new HtmlWeb();

        public async Task<List<ApiVehicle>> GetAllVehicles()
        {
            var allVehicles = new List<ApiVehicle>();

            for (var i = 0; i < 16; i++)
            {
                var page = 0;
                var b = new BitArray(new[] { i });
                List<ApiVehicle> resVehicles;
                do
                {
                    resVehicles = await GetVehiclesAsync(200, page, b[0], b[1], b[2], b[3]);
                    allVehicles.AddRange(resVehicles);
                    page++;
                } while (resVehicles.Count == 200);
            }

            return allVehicles;
        }

        private async Task<List<ApiVehicle>> GetVehiclesAsync(int limit, int page, bool isPremium, bool isPack, bool isSquadronVehicle,
            bool isOnMarketplace)
        {
            try
            {
                var response = await _apiClient.GetAsync(
                    $"vehicles?limit={limit}&page={page}&isPremium={isPremium.ToString().ToLowerInvariant()}&isPack={isPack.ToString().ToLowerInvariant()}&isSquadronVehicle={isSquadronVehicle.ToString().ToLowerInvariant()}&isOnMarketplace={isOnMarketplace.ToString().ToLowerInvariant()}&excludeKillstreak=true&excludeEventVehicles=true");

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<List<ApiVehicle>>(await response.Content.ReadAsStringAsync()) ?? [];
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return [];
            }
        }

        public List<VehicleIdentifier> GetIdentifierTranslation()
        {
            var document = _web.Load(config.GetConfig("Scraper", "VehicleCDK-URL") ?? "");
            var elements = document.DocumentNode.SelectNodes("//table[@class='wikitable sortable mw-collapsible mw-collapsed']");

            return elements
                .Select(
                    n => n.ChildNodes
                        .Where(cn => cn.Name == "tr")
                        .Skip(1)
                        .Select(cnn => cnn.InnerText)
                    )
                .SelectMany(tr => tr)
                .Select(s => s.Split('\n'))
                .Select(sl => new VehicleIdentifier
                {
                    Id = sl[1].Trim(),
                    Name = sl[2].Trim()

                })
                .ToList();
        }
    }
}
