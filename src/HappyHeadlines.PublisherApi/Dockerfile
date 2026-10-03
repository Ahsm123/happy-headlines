FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/HappyHeadlines.PublisherApi/HappyHeadlines.PublisherApi.csproj src/HappyHeadlines.PublisherApi/
COPY Directory.Build.props ./
COPY src/HappyHeadlines.Contracts/HappyHeadlines.Contracts.csproj src/HappyHeadlines.Contracts/
COPY src/HappyHeadlines.ServiceDefaults/HappyHeadlines.ServiceDefaults.csproj src/HappyHeadlines.ServiceDefaults/
RUN dotnet restore src/HappyHeadlines.PublisherApi/HappyHeadlines.PublisherApi.csproj
COPY src/HappyHeadlines.PublisherApi/ src/HappyHeadlines.PublisherApi/
COPY src/HappyHeadlines.Contracts/ src/HappyHeadlines.Contracts/
COPY src/HappyHeadlines.ServiceDefaults/ src/HappyHeadlines.ServiceDefaults/
RUN dotnet publish src/HappyHeadlines.PublisherApi/HappyHeadlines.PublisherApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HappyHeadlines.PublisherApi.dll"]