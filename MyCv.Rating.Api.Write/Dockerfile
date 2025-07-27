#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MyCv.Rating.Api.Write/MyCv.Rating.Api.Write.csproj", "MyCv.Rating.Api.Write/"]
COPY ["common/MyCv.Common.Domain/MyCv.Common.Domain.csproj", "common/MyCv.Common.Domain/"]
COPY ["common/MyCv.Common.HealthChecks/MyCv.Common.HealthChecks.csproj", "common/MyCv.Common.HealthChecks/"]
COPY ["rating/MyCv.Rating.Application/MyCv.Rating.Application.csproj", "rating/MyCv.Rating.Application/"]
COPY ["rating/MyCv.Rating.Infrastructure/MyCv.Rating.Infrastructure.csproj", "rating/MyCv.Rating.Infrastructure/"]
COPY ["rating/MyCv.Rating.Domain/MyCv.Rating.Domain.csproj", "rating/MyCv.Rating.Domain/"]
RUN dotnet restore "./MyCv.Rating.Api.Write/MyCv.Rating.Api.Write.csproj"
COPY . .
WORKDIR "/src/MyCv.Rating.Api.Write"
RUN dotnet build "./MyCv.Rating.Api.Write.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyCv.Rating.Api.Write.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyCv.Rating.Api.Write.dll"]