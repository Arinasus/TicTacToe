using TicTacToe.Server.Hubs;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://localhost:7255",   
            "http://localhost:5286",
            "https://remarkable-fairy-da7f88.netlify.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseCors();

app.UseRouting();
app.MapControllers();
app.MapRazorPages();

app.MapHub<GameHub>("/gamehub");

app.Run();
