# 1) Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 2) Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AADBOT-KarloHruskovec/AADBOT-KarloHruskovec.csproj", "AADBOT-KarloHruskovec/"]
RUN dotnet restore "AADBOT-KarloHruskovec/AADBOT-KarloHruskovec.csproj"
COPY . .
WORKDIR "/src/AADBOT-KarloHruskovec"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 3) Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AADBOT-KarloHruskovec.dll"]
