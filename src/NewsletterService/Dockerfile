FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/NewsletterService/NewsletterService.csproj src/NewsletterService/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/NewsletterService/NewsletterService.csproj
COPY src/NewsletterService/ src/NewsletterService/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/NewsletterService/NewsletterService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "NewsletterService.dll"]
