FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/PublisherService/PublisherService.csproj src/PublisherService/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/PublisherService/PublisherService.csproj
COPY src/PublisherService/ src/PublisherService/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/PublisherService/PublisherService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PublisherService.dll"]