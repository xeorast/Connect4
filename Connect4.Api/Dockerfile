#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Connect4.Api/Connect4.Api.csproj", "Connect4.Api/"]
COPY ["Connect4.Domain/Connect4.Domain.csproj", "Connect4.Domain/"]
COPY ["Connect4.Migrations.MsSql/Connect4.Migrations.MsSql.csproj", "Connect4.Migrations.MsSql/"]
COPY ["Connect4.Data/Connect4.Data.csproj", "Connect4.Data/"]
COPY ["Connect4.Api.Shared/Connect4.Api.Shared.csproj", "Connect4.Api.Shared/"]
COPY ["Connect4.Migrations.Pg/Connect4.Migrations.Pg.csproj", "Connect4.Migrations.Pg/"]
COPY ["Connect4.Multiplayer/Connect4.Multiplayer.csproj", "Connect4.Multiplayer/"]
RUN dotnet restore "Connect4.Api/Connect4.Api.csproj"
COPY . .
WORKDIR "/src/Connect4.Api"
RUN dotnet build "Connect4.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Connect4.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Connect4.Api.dll"]