using System.Configuration;
using System.Net.Http.Json;
using WTBattleExtractor.Dto.WtSbAssistant;

namespace WTBattleExtractor.APIs.WebServer
{
    public class WtSbServer
    {
        private readonly HttpClient? _client;

        public WtSbServer()
        {
            if (ConfigurationManager.AppSettings["WebServer"] is not { } url) return;
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task PostLogsAsync(WtLog log)
        {
            if (_client == null) return;

            try
            {
                var response = await _client.PostAsJsonAsync("WtLog", log);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
