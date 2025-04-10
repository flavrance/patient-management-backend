FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Install curl for health checks - ensure it works with a retry
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/FinX.API/FinX.API.csproj", "src/FinX.API/"]
COPY ["src/FinX.Application/FinX.Application.csproj", "src/FinX.Application/"]
COPY ["src/FinX.Domain/FinX.Domain.csproj", "src/FinX.Domain/"]
COPY ["src/FinX.Infrastructure/FinX.Infrastructure.csproj", "src/FinX.Infrastructure/"]
RUN dotnet restore "src/FinX.API/FinX.API.csproj"
COPY . .
WORKDIR "/src/src/FinX.API"
RUN dotnet build "FinX.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinX.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinX.API.dll"] 