FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/LinkTamer.Api/LinkTamer.Api.csproj", "src/LinkTamer.Api/"]
COPY ["src/LinkTamer.Application/LinkTamer.Application.csproj", "src/LinkTamer.Application/"]
COPY ["src/LinkTamer.Domain/LinkTamer.Domain.csproj", "src/LinkTamer.Domain/"]
COPY ["src/LinkTamer.Infrastructure/LinkTamer.Infrastructure.csproj", "src/LinkTamer.Infrastructure/"]
RUN dotnet restore "src/LinkTamer.Api/LinkTamer.Api.csproj"
COPY . .
WORKDIR "/src/src/LinkTamer.Api"
RUN dotnet build "LinkTamer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinkTamer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkTamer.Api.dll"]