FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../src/TruLayer.Pokedex.Api/TruLayer.Pokedex.Api.csproj", "src/TruLayer.Pokedex.Api/"]
RUN dotnet restore "src/TruLayer.Pokedex.Api/TruLayer.Pokedex.Api.csproj"
COPY . .
WORKDIR "/src/src/TruLayer.Pokedex.Api"
RUN dotnet build "TruLayer.Pokedex.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TruLayer.Pokedex.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TruLayer.Pokedex.Api.dll"]