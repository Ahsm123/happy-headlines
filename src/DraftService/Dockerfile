FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/DraftService/DraftService.csproj src/DraftService/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/DraftService/DraftService.csproj
COPY src/DraftService/ src/DraftService/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/DraftService/DraftService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DraftService.dll"]
