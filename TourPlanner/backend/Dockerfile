# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime (Corectat pentru driverul de Postgres în .NET 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Instalăm dependența nativă pentru Kerberos/GSSAPI cerută de Npgsql
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "backend.dll"]