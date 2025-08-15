# Build runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/IdentityAccess.Domain/IdentityAccess.Domain.csproj ./src/IdentityAccess.Domain/
COPY ./src/IdentityAccess.Infrastructure/IdentityAccess.Infrastructure.csproj ./src/IdentityAccess.Infrastructure/
COPY ./src/IdentityAccess.Api/IdentityAccess.Api.csproj ./src/IdentityAccess.Api/
RUN dotnet restore ./src/IdentityAccess.Api/IdentityAccess.Api.csproj

COPY . .
RUN dotnet publish ./src/IdentityAccess.Api/IdentityAccess.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityAccess.Api.dll"]