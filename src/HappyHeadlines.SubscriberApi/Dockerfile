FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/HappyHeadlines.SubscriberApi/HappyHeadlines.SubscriberApi.csproj src/HappyHeadlines.SubscriberApi/
COPY Directory.Build.props ./
COPY src/HappyHeadlines.Contracts/HappyHeadlines.Contracts.csproj src/HappyHeadlines.Contracts/
COPY src/HappyHeadlines.ServiceDefaults/HappyHeadlines.ServiceDefaults.csproj src/HappyHeadlines.ServiceDefaults/
RUN dotnet restore src/HappyHeadlines.SubscriberApi/HappyHeadlines.SubscriberApi.csproj
COPY src/HappyHeadlines.SubscriberApi/ src/HappyHeadlines.SubscriberApi/
COPY src/HappyHeadlines.Contracts/ src/HappyHeadlines.Contracts/
COPY src/HappyHeadlines.ServiceDefaults/ src/HappyHeadlines.ServiceDefaults/
RUN dotnet publish src/HappyHeadlines.SubscriberApi/HappyHeadlines.SubscriberApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HappyHeadlines.SubscriberApi.dll"]
