using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TicTacToeApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton(sp =>
    new HubConnectionBuilder()
    .WithUrl("https://localhost:7261/gamehub")
    .WithAutomaticReconnect()
    .Build());
await builder.Build().RunAsync(); 
