# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ideabench.app.slnx ./
COPY src/Ideabench.Data/Ideabench.Data.csproj src/Ideabench.Data/
COPY src/Ideabench.Endpoints/Ideabench.Endpoints.csproj src/Ideabench.Endpoints/
COPY src/Ideabench.Ui/Ideabench.Ui.csproj src/Ideabench.Ui/
COPY src/Ideabench.DbMigrator/Ideabench.DbMigrator.csproj src/Ideabench.DbMigrator/

RUN dotnet restore ideabench.app.slnx

COPY src/ ./src/

RUN dotnet publish src/Ideabench.Ui/Ideabench.Ui.csproj -c Release -o /app/ui
RUN dotnet publish src/Ideabench.DbMigrator/Ideabench.DbMigrator.csproj -c Release -o /app/migrator

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS ui
WORKDIR /app
COPY --from=build /app/ui ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ideabench.Ui.dll"]

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS migrator
WORKDIR /app
COPY --from=build /app/migrator ./
ENTRYPOINT ["dotnet", "Ideabench.DbMigrator.dll"]
