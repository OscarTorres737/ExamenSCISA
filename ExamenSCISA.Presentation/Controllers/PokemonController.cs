using ExamenSCISA.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamenSCISA.Presentation.Controllers
{
    public class PokemonController : Controller
    {
        private readonly IPokemonService _pokemonServicio;

        public PokemonController(IPokemonService pokemonServicio)
        {
            _pokemonServicio = pokemonServicio;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lista = await _pokemonServicio.ObtenerPokemones(0, 20); // primeros 20 regsitros
                return View(lista); // pasamos la data a la vista
            }
            catch (Exception ex)
            {
                throw new Exception("hubo un error al traer pokemones: " + ex.Message);
            }
        }
    }
}
