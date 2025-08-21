namespace ExamenSCISA.Application.Interfaces
{
    public interface ITipoService
    {
        Task<List<string>> ObtenerTipos();
    }
}
