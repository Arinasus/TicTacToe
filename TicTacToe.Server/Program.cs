using TicTacToe.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7255", "http://localhost:5286")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();

    });
});

var app = builder.Build();

app.UseCors(); // включаем CORS

app.MapHub<GameHub>("/gamehub");

app.MapGet("/", () => "TicTacToe Server запущен!");

app.Run();
