FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy sln + csproj and restore as distinct layers
COPY Directory.Build.props Supervisor.sln ./
COPY src/Supervisor.csproj ./src/
COPY UnitTests/Supervisor.UnitTests.csproj ./UnitTests/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet test -c Release
RUN dotnet publish src -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

RUN curl -L "https://github.com/docker/compose/releases/download/1.26.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

RUN chmod +x /usr/local/bin/docker-compose

RUN apt-get update && apt-get install -y git && apt-get install

COPY --from=build-env /app/out .

COPY ./secret_webhook /app/secret_webhook

EXPOSE 3000/tcp

ENTRYPOINT ["dotnet", "Supervisor.dll"]
