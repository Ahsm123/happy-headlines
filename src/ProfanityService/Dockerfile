FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ProfanityService/ProfanityService.csproj src/ProfanityService/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/ProfanityService/ProfanityService.csproj
COPY src/ProfanityService/ src/ProfanityService/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/ProfanityService/ProfanityService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProfanityService.dll"]
