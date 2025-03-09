using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace syscom.Services
{
    public class DiasFestivosService
    {
        private readonly HttpClient _httpClient;

        public DiasFestivosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DateTime>> ObtenerDiasFestivosAsync()
        {
            string url = "https://api-colombia.com/api/v1/holiday/year/2025";
            var response = await _httpClient.GetStringAsync(url);
            var festivos = JsonSerializer.Deserialize<List<Festivo>>(response);

            return festivos?.Select(f => f.Date).ToList() ?? new List<DateTime>();
        }
    }
    public class Festivo
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
