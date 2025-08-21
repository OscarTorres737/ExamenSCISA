using ClosedXML.Excel;
using ExamenSCISA.Application;
using ExamenSCISA.Application.Interfaces;
using System.Reflection;

namespace ExamenSCISA.Infrastructure.Services
{
    public class ExportarExcelService : IExportarExcelService
    {
        //contruir excel en base a lista recibida
        public byte[] GenerarExcel(List<PokemonDto> pokemones)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Pokemones");

            if (pokemones == null || !pokemones.Any())
                return Array.Empty<byte>();

            var propiedades = typeof(PokemonDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // titulos dinámicos
            for (int i = 0; i < propiedades.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = propiedades[i].Name;
            }

            int fila = 2;
            foreach (var p in pokemones)
            {
                for (int i = 0; i < propiedades.Length; i++)
                {
                    var valor = propiedades[i].GetValue(p);

                    if (valor is IEnumerable<string> lista)
                        worksheet.Cell(fila, i + 1).Value = string.Join(", ", lista);
                    else
                        worksheet.Cell(fila, i + 1).Value = valor?.ToString() ?? "";
                }
                fila++;
            }

            worksheet.Columns().AdjustToContents();
            worksheet.Row(1).Style.Font.Bold = true;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
