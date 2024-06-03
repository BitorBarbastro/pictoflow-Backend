# Usa una imagen base adecuada
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Exponer los puertos necesarios
EXPOSE 3000
EXPOSE 5046
EXPOSE 80
EXPOSE 443
# Usa una imagen para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["pictoflow-Backend.csproj", "."]
RUN dotnet restore "./pictoflow-Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./pictoflow-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./pictoflow-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN mkdir -p /app/uploads/

COPY uploads /app/uploads

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "pictoflow-Backend.dll"]
