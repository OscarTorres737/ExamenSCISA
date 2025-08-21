using ExamenSCISA.Application;
using ExamenSCISA.Application.Interfaces;
using System.Text.Json;

namespace ExamenSCISA.Infrastructure.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly HttpClient _http;

        public PokemonService(HttpClient http)
        {
            _http = http;
        }

        // nombre, especie y img para tabla inicial
        public async Task<List<PokemonDto>> ObtenerPokemones(int pagina, int tamañoPagina, string nombrefiltro = "", string especiefiltro = "")
        {
            var pokemones = new List<PokemonDto>();
            string url = string.IsNullOrEmpty(nombrefiltro)
                ? $"https://pokeapi.co/api/v2/pokemon?offset={(pagina - 1) * tamañoPagina}&limit={tamañoPagina}"
                : $"https://pokeapi.co/api/v2/pokemon/{nombrefiltro.ToLower()}";

            try
            {
                var respuesta = await _http.GetAsync(url);
                respuesta.EnsureSuccessStatusCode();
                var contenido = await respuesta.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(contenido);

                if (json.RootElement.TryGetProperty("results", out var results))
                {
                    // pagnacion normal
                    foreach (var item in results.EnumerateArray())
                    {
                        string nombre = item.GetProperty("name").GetString() ?? "";
                        string detalleUrl = item.GetProperty("url").GetString() ?? "";
                        string imagen = "";
                        var tipos = new List<string>();

                        try
                        {
                            var detalleResp = await _http.GetAsync(detalleUrl);
                            detalleResp.EnsureSuccessStatusCode();
                            var detalleStr = await detalleResp.Content.ReadAsStringAsync();
                            var detalleJson = JsonDocument.Parse(detalleStr);

                            imagen = detalleJson.RootElement.GetProperty("sprites").GetProperty("front_default").GetString() ?? "";

                            // tipos 
                            foreach (var tipoElement in detalleJson.RootElement.GetProperty("types").EnumerateArray())
                            {
                                string tipoNombre = tipoElement.GetProperty("type").GetProperty("name").GetString() ?? "";
                                tipos.Add(tipoNombre);
                            }
                        }
                        catch
                        {
                            imagen = "";
                        }

                        pokemones.Add(new PokemonDto
                        {
                            Nombre = nombre,
                            ImagenUrl = imagen,
                            Tipos = tipos
                        });
                    }
                }
                else
                {
                    // pokemon individual por nombre
                    string nombre = json.RootElement.GetProperty("name").GetString() ?? "";
                    string imagen = json.RootElement.GetProperty("sprites").GetProperty("front_default").GetString() ?? "";
                    var tipos = new List<string>();

                    foreach (var tipoElement in json.RootElement.GetProperty("types").EnumerateArray())
                    {
                        string tipoNombre = tipoElement.GetProperty("type").GetProperty("name").GetString() ?? "";
                        tipos.Add(tipoNombre);
                    }

                    pokemones.Add(new PokemonDto
                    {
                        Nombre = nombre,
                        ImagenUrl = imagen,
                        Tipos = tipos
                    });
                }

                // filtrar por tipo
                if (!string.IsNullOrWhiteSpace(especiefiltro))
                {
                    pokemones = pokemones
                        .Where(p => p.Tipos.Contains(especiefiltro, StringComparer.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            catch
            {
                pokemones = new List<PokemonDto>();
            }

            return pokemones;
        }

        //obtiene detalle del pokemon
        public async Task<PokemonDto> ObtenerDetalle(string nombre)
        {
            string url = $"https://pokeapi.co/api/v2/pokemon/{nombre.ToLower()}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var contenido = await resp.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(contenido);

            int id = json.RootElement.GetProperty("id").GetInt32();
            string especie = json.RootElement.GetProperty("species").GetProperty("name").GetString() ?? "";
            int altura = json.RootElement.GetProperty("height").GetInt32();
            int peso = json.RootElement.GetProperty("weight").GetInt32();
            string imagen = json.RootElement.GetProperty("sprites").GetProperty("front_default").GetString() ?? "";

            var habilidades = new List<string>();
            foreach (var abilityElement in json.RootElement.GetProperty("abilities").EnumerateArray())
            {
                string habilidad = abilityElement.GetProperty("ability").GetProperty("name").GetString() ?? "";
                if (!string.IsNullOrEmpty(habilidad))
                    habilidades.Add(habilidad);
            }

            var tipos = new List<string>();
            foreach (var tipoElement in json.RootElement.GetProperty("types").EnumerateArray())
            {
                string tipoNombre = tipoElement.GetProperty("type").GetProperty("name").GetString() ?? "";
                tipos.Add(tipoNombre);
            }

            return new PokemonDto
            {
                Id = id,
                Nombre = nombre,
                Especie = especie,
                Altura = altura,
                Peso = peso,
                ImagenUrl = imagen,
                Habilidades = habilidades,
                Tipos = tipos
            };
        }
    }
}
