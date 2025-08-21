using ExamenSCISA.Application;
using ExamenSCISA.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExamenSCISA.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPokemonService _pokemonServicio;
        private readonly ITipoService _tipoService;
        private readonly IExportarExcelService _excelServicio;
        private readonly ICorreoService _correoServicio;

        public IndexModel(
            IPokemonService pokemonServicio,
            ITipoService tiposervice,
            IExportarExcelService excelServicio,
            ICorreoService correoServicio)
        {
            _pokemonServicio = pokemonServicio;
            _tipoService = tiposervice;
            _excelServicio = excelServicio;
            _correoServicio = correoServicio;
        }

        // filtros
        [BindProperty(SupportsGet = true)]
        public string NombreFiltro { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public string EspecieFiltro { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 21; // cuantos pokemones por pg
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }

        // datos para la vista
        public List<PokemonDto> Pokemones { get; set; } = new();

        public List<string> TiposElementales { get; set; } = new();


        public async Task OnGetAsync()
        {
            try
            {
                TiposElementales = await _tipoService.ObtenerTipos();

                // cantidad de registros por página
                int registrosPorPagina = TamañoPagina;

                // obtenemos los pokemones de la página actual
                Pokemones = await _pokemonServicio.ObtenerPokemones(Pagina, registrosPorPagina, NombreFiltro, EspecieFiltro);

                // asignamos la página actual
                PaginaActual = Pagina;
            }
            catch (Exception)
            {
                Pokemones = new List<PokemonDto>();
                PaginaActual = 1;
            }
        }
        [BindProperty]
        public PokemonDto PokemonDetalle { get; set; } = new();

        public async Task<IActionResult> OnGetDetalleAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return NotFound();

            PokemonDetalle = await _pokemonServicio.ObtenerDetalle(nombre);

            return new JsonResult(PokemonDetalle);
        }

        // exportar Excel
        public async Task<IActionResult> OnPostExportarExcel()
        {
            try
            {
                var pokemones = await _pokemonServicio.ObtenerPokemones(Pagina, TamañoPagina, NombreFiltro, EspecieFiltro);

                var detalles = new List<PokemonDto>();
                foreach (var p in pokemones)
                {
                    var detalle = await _pokemonServicio.ObtenerDetalle(p.Nombre);
                    if (detalle != null)
                        detalles.Add(detalle);
                }

                var excelBytes = _excelServicio.GenerarExcel(detalles);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pokemones.xlsx");
            }
            catch (Exception)
            {
                return RedirectToPage();
            }
        }

        // enviar por correo
        public async Task<IActionResult> OnPostEnviarCorreoAsync(string correoDestino, string asuntoCorreo)
        {
            if (string.IsNullOrWhiteSpace(correoDestino))
            {
                TempData["Error"] = "Debe ingresar al menos un correo destino.";
                return RedirectToPage();
            }

            // separa correos por coma 
            var destinatarios = correoDestino
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            if (!destinatarios.Any())
            {
                TempData["Error"] = "No se encontraron correos válidos.";
                return RedirectToPage();
            }

            try
            {
                var pokemones = await _pokemonServicio.ObtenerPokemones(Pagina, TamañoPagina, NombreFiltro, EspecieFiltro);

                var detalles = new List<PokemonDto>();
                foreach (var p in pokemones)
                {
                    var detalle = await _pokemonServicio.ObtenerDetalle(p.Nombre);
                    if (detalle != null)
                        detalles.Add(detalle);
                }

                // genera exsel solo con los pokemones visibles en pantalla
                var excelBytes = _excelServicio.GenerarExcel(detalles);

                foreach (var destinatario in destinatarios)
                {
                    await _correoServicio.EnviarCorreo(
                        destinatario,
                        string.IsNullOrWhiteSpace(asuntoCorreo) ? "Pokemones" : asuntoCorreo,
                        "Adjunto el listado de pokemones visibles en pantalla",
                        excelBytes,
                        "Pokemones.xlsx"
                    );
                }

                TempData["Mensaje"] = $"Correo(s) enviado(s) correctamente a {string.Join(", ", destinatarios)}";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al enviar el correo: " + ex.Message;
                return RedirectToPage();
            }
        }



    }
}
