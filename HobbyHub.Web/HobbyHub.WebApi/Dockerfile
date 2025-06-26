FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HobbyHub.WebApi/HobbyHub.WebApi.csproj", "HobbyHub.WebApi/"]
COPY ["HobbyHub.UserIdentity.Provider.Abstractions/HobbyHub.UserIdentity.Provider.Abstractions.csproj", "HobbyHub.UserIdentity.Provider.Abstractions/"]
COPY ["HobbyHub.UserIdentity.Provider/HobbyHub.UserIdentity.Provider.csproj", "HobbyHub.UserIdentity.Provider/"]
COPY ["HobbyHub.S3.Driver.Abstractions/HobbyHub.S3.Driver.Abstractions.csproj", "HobbyHub.S3.Driver.Abstractions/"]
COPY ["HobbyHub.S3.Driver/HobbyHub.S3.Driver.csproj", "HobbyHub.S3.Driver/"]
COPY ["HobbyHub.Application/HobbyHub.Application.csproj", "HobbyHub.Application/"]
COPY ["HobbyHub.Database/HobbyHub.Database.csproj", "HobbyHub.Database/"]
COPY ["HobbyHub.Contract/HobbyHub.Contract.csproj", "HobbyHub.Contract/"]
COPY ["HobbyHub.Database.Infrastructure/HobbyHub.Database.Infrastructure.csproj", "HobbyHub.Database.Infrastructure/"]
COPY ["HobbyHub.Infrastructure/HobbyHub.Infrastructure.csproj", "HobbyHub.Infrastructure/"]
COPY ["HobbyHub.Repository/HobbyHub.Repository.csproj", "HobbyHub.Repository/"]
COPY ["HobbyHub.Repository.Abstractions/HobbyHub.Repository.Abstractions.csproj", "HobbyHub.Repository.Abstractions/"]
RUN dotnet restore "HobbyHub.WebApi/HobbyHub.WebApi.csproj"
COPY . .
WORKDIR "/src/HobbyHub.WebApi"
RUN dotnet build "HobbyHub.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HobbyHub.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HobbyHub.WebApi.dll"]
