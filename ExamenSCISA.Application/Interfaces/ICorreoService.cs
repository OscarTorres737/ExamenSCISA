namespace ExamenSCISA.Application.Interfaces
{
    public interface ICorreoService
    {
        // envia correo con el excel adjunto a uno o varios correos
        Task EnviarCorreo(string para, string asunto, string cuerpo, byte[] archivoAdjunto, string nombreArchivo);
    }
}
