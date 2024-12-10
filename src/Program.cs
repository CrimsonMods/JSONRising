using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using JSONRising;
using JSONRising.Models;
using JSONRising.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<UpdateService>();
builder.Services.AddScoped<ISchemaService, SchemaService>();
builder.Services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Debug));

await builder.Build().RunAsync();
