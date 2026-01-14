FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/NutriTrack.Notifications.Worker/NutriTrack.Notifications.Worker.csproj
RUN dotnet publish src/NutriTrack.Notifications.Worker/NutriTrack.Notifications.Worker.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NutriTrack.Notifications.Worker.dll"]
