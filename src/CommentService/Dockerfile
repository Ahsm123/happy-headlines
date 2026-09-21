FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/CommentService/CommentService.csproj src/CommentService/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
RUN dotnet restore src/CommentService/CommentService.csproj
COPY src/CommentService/ src/CommentService/
COPY src/ServiceDefaults/ src/ServiceDefaults/
RUN dotnet publish src/CommentService/CommentService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CommentService.dll"]
