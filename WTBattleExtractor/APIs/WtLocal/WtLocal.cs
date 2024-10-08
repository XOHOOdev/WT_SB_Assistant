using Newtonsoft.Json;
using System.Configuration;
using WTBattleExtractor.Dto;

namespace WTBattleExtractor.APIs.WtLocal
{
    public class WtLocal
    {
        private readonly HttpClient? _client;
        private readonly HttpClient? _watchClient;
        private int _lastMessage;

        public WtLocal()
        {
            if (ConfigurationManager.AppSettings["WtApi"] is { } url)
            {
                _client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
                _watchClient = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
            }
        }

        public async Task<Root?> GetLogsAsync()
        {
            if (_client == null) return null;

            try
            {
                using var response = await _client.GetAsync($"hudmsg?lastEvt=0&lastDmg={_lastMessage}");
                if (response.IsSuccessStatusCode)
                {
                    var resString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Root>(resString);

                    if (res == null) return null;

                    if (res.Damage.LastOrDefault() is { } dmg)
                    {
                        _lastMessage = dmg.Id;
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<bool> MatchFinishedAsync()
        {
            if (_watchClient == null) return false;

            try
            {
                using var response = await _watchClient.GetAsync("map_obj.json");
                if (response.IsSuccessStatusCode)
                {
                    return string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
