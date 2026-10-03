FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/HappyHeadlines.CommentApi/HappyHeadlines.CommentApi.csproj src/HappyHeadlines.CommentApi/
COPY Directory.Build.props ./
COPY src/HappyHeadlines.Contracts/HappyHeadlines.Contracts.csproj src/HappyHeadlines.Contracts/
COPY src/HappyHeadlines.ServiceDefaults/HappyHeadlines.ServiceDefaults.csproj src/HappyHeadlines.ServiceDefaults/
RUN dotnet restore src/HappyHeadlines.CommentApi/HappyHeadlines.CommentApi.csproj
COPY src/HappyHeadlines.CommentApi/ src/HappyHeadlines.CommentApi/
COPY src/HappyHeadlines.Contracts/ src/HappyHeadlines.Contracts/
COPY src/HappyHeadlines.ServiceDefaults/ src/HappyHeadlines.ServiceDefaults/
RUN dotnet publish src/HappyHeadlines.CommentApi/HappyHeadlines.CommentApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HappyHeadlines.CommentApi.dll"]
