using ExamenSCISA.Application.Interfaces;
using ExamenSCISA.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// servicios
builder.Services.AddHttpClient<IPokemonService, PokemonService>();
builder.Services.AddHttpClient<ITipoService, TipoService>();
builder.Services.AddScoped<IExportarExcelService, ExportarExcelService>();
builder.Services.AddScoped<ICorreoService, CorreoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
