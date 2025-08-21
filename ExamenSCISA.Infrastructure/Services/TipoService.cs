using ExamenSCISA.Application.Interfaces;
using System.Text.Json;

namespace ExamenSCISA.Infrastructure.Services
{
    public class TipoService : ITipoService
    {
        private readonly HttpClient _http;

        public TipoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<string>> ObtenerTipos()
        {
            var response = await _http.GetAsync("https://pokeapi.co/api/v2/type");
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var results = doc.RootElement.GetProperty("results");
            var tipos = new List<string>();

            foreach (var r in results.EnumerateArray())
            {
                tipos.Add(r.GetProperty("name").GetString()!);
            }

            return tipos;
        }
    }
}
