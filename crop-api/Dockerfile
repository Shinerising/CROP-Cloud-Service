# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj ./crop-api/
RUN dotnet restore ./crop-api/

# copy everything else and build app
COPY . ./crop-api/
WORKDIR /source/crop-api
RUN dotnet publish -c release -o /crop-api-app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /crop-api-app
COPY --from=build /crop-api-app ./

# install vmstat, ifstat, free, lshw
RUN apt-get update && apt-get install -y lshw procps ifstat

# add entrypoint
ENTRYPOINT ["dotnet", "CROP API.dll"]

# add ports
EXPOSE 5182

# add volumes
VOLUME [ "/storage", "/logs", "/seeding" ]

# add environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://localhost:5182
ENV DATABASE_URL=postgres://postgres:postgres@postgres:5432/postgres
ENV REDIS_URL=redis://redis:6379
ENV SECRET_KEY=secret
