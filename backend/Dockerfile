FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

# Instalăm dependența nativă pentru Kerberos/GSSAPI cerută de Npgsql
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY *.csproj ./
RUN dotnet restore

COPY . ./

EXPOSE 8080

# Pornim backend-ul cu watch pentru a compila automat la salvare
CMD ["dotnet", "watch", "run", "--urls=http://+:8080"]