FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/NutriTrack.Notifications.Api/NutriTrack.Notifications.Api.csproj
RUN dotnet publish src/NutriTrack.Notifications.Api/NutriTrack.Notifications.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NutriTrack.Notifications.Api.dll"]
