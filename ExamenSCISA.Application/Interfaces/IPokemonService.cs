namespace ExamenSCISA.Application.Interfaces
{
    public interface IPokemonService
    {
        // obtiene pokemones paginados, con opcion de filtrar por nombre y especie
        Task<List<PokemonDto>> ObtenerPokemones(int pagina, int tamañoPagina, string nombrefiltro = "", string especiefiltro = "");
        Task<PokemonDto> ObtenerDetalle(string nombre);
    }
}
