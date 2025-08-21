namespace ExamenSCISA.Application.Interfaces
{
    public interface IExportarExcelService
    {
        // genera un archivo excel a partir de una lista de pokemones
        byte[] GenerarExcel(List<PokemonDto> pokemones);
    }
}
