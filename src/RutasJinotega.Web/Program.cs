using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Shared.UI;
using RutasJinotega.Shared.UI.Servicios;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Raíz de composición de la app web: toda la UI vive en Shared.UI.
builder.RootComponents.Add<Routes>("#app");

var urlApi = builder.Configuration["UrlApi"] ?? "http://localhost:5100/";
builder.Services.AgregarClienteApi(new Uri(urlApi));
builder.Services.AddScoped<IServicioUbicacion, ServicioUbicacionWeb>();

await builder.Build().RunAsync();
