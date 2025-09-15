FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MeoMeoAPI/MeoMeo.API.csproj", "MeoMeoAPI/"]
COPY ["MeoMeo.Contract/MeoMeo.Contract.csproj", "MeoMeo.Contract/"]
COPY ["MeoMeoDomain/MeoMeo.Domain.csproj", "MeoMeoDomain/"]
COPY ["MeoMeo.EntityFrameworkCore/MeoMeo.EntityFrameworkCore.csproj", "MeoMeo.EntityFrameworkCore/"]
COPY ["MeoMeoApplication/MeoMeo.Application.csproj", "MeoMeoApplication/"]
COPY ["MeoMeo.Shared/MeoMeo.Shared.csproj", "MeoMeo.Shared/"]
RUN dotnet restore "MeoMeoAPI/MeoMeo.API.csproj"
COPY . .
WORKDIR "/src/MeoMeoAPI"
RUN dotnet build "./MeoMeo.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MeoMeo.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeoMeo.API.dll"]
