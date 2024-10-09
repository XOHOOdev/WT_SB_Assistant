using Newtonsoft.Json;
using System.Configuration;
using WTBattleExtractor.Dto.WtLocal;

namespace WTBattleExtractor.APIs.WtLocal
{
    public class WtLocal
    {
        private readonly HttpClient? _client;
        private readonly HttpClient? _watchClient;
        private int _lastMessage;

        public WtLocal()
        {
            if (ConfigurationManager.AppSettings["WtApi"] is not { } url) return;
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            _watchClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task<WtLocalRoot?> GetLogsAsync()
        {
            if (_client == null) return null;

            try
            {
                using var response = await _client.GetAsync($"hudmsg?lastEvt=0&lastDmg={_lastMessage}");
                var responseTime = DateTime.Now;

                if (response.IsSuccessStatusCode)
                {
                    var resString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<WtLocalRoot>(resString);

                    if (res == null) return null;

                    if (res.Damage.LastOrDefault() is not { } dmg) return res;

                    res.Time = responseTime.Subtract(TimeSpan.FromSeconds(dmg.Time));
                    _lastMessage = dmg.Id;
                    return res;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<string?> GetMatchResult()
        {
            if (_client == null) return null;

            try
            {
                using var response = await _client.GetAsync("mission.json");
                if (response.IsSuccessStatusCode)
                {
                    var resString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<MissionRoot>(resString);

                    return res?.Status;
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
                using var response = await _watchClient.GetAsync("map_info.json");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<MapRoot>(jsonString);

                    return !res?.Valid ?? false;
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
