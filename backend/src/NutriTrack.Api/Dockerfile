# ===== RUN image (lightweight) =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# ===== BUILD image =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore src/NutriTrack.Api/NutriTrack.Api.csproj
RUN dotnet publish src/NutriTrack.Api/NutriTrack.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== FINAL image =====
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NutriTrack.Api.dll"]
