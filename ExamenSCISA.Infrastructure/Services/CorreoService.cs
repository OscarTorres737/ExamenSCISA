using ExamenSCISA.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ExamenSCISA.Infrastructure.Services
{
    public class CorreoService : ICorreoService
    {
        private readonly string _apiKey = "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";

        public async Task EnviarCorreo(string para, string asunto, string cuerpo, byte[] archivoAdjunto, string nombreArchivo)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("oscarariel.torres737@gmail.com", "Oscar");
            var to = new EmailAddress(para);
            var msg = MailHelper.CreateSingleEmail(from, to, asunto, cuerpo, cuerpo);

            if (archivoAdjunto != null)
            {
                msg.AddAttachment(nombreArchivo, Convert.ToBase64String(archivoAdjunto));
            }

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Error enviando correo: {response.StatusCode}");
            }
        }
    }
}
