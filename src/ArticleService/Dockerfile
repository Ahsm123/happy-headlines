FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ArticleService/ArticleService.csproj src/ArticleService/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/ArticleService/ArticleService.csproj
COPY src/ArticleService/ src/ArticleService/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/ArticleService/ArticleService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ArticleService.dll"]
