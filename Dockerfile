FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY TicTacToeApp/TicTacToeApp.csproj TicTacToeApp/
COPY TicTacToe.Server/TicTacToe.Server.csproj TicTacToe.Server/
COPY TicTacToe.Shared/TicTacToe.Shared.csproj TicTacToe.Shared/
RUN dotnet restore

COPY . .
RUN dotnet publish TicTacToe.Server/TicTacToe.Server.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.Server.dll"]
