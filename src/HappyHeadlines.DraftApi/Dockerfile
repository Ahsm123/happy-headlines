FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/HappyHeadlines.DraftApi/HappyHeadlines.DraftApi.csproj src/HappyHeadlines.DraftApi/
COPY Directory.Build.props ./
COPY src/HappyHeadlines.ServiceDefaults/HappyHeadlines.ServiceDefaults.csproj src/HappyHeadlines.ServiceDefaults/
RUN dotnet restore src/HappyHeadlines.DraftApi/HappyHeadlines.DraftApi.csproj
COPY src/HappyHeadlines.DraftApi/ src/HappyHeadlines.DraftApi/
COPY src/HappyHeadlines.ServiceDefaults/ src/HappyHeadlines.ServiceDefaults/
RUN dotnet publish src/HappyHeadlines.DraftApi/HappyHeadlines.DraftApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HappyHeadlines.DraftApi.dll"]
