# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj ./crop-api/
RUN dotnet restore

# copy everything else and build app
COPY * ./crop-api/
WORKDIR /source/crop-api
RUN dotnet publish -c release -o /crop-api-app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /crop-api-app
COPY --from=build /crop-api-app ./
ENTRYPOINT ["dotnet", "CROP API.dll"]
