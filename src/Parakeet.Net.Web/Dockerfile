#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Parakeet.Net.Web/Parakeet.Net.Web.csproj", "src/Parakeet.Net.Web/"]
COPY ["src/Parakeet.Net.Application/Parakeet.Net.Application.csproj", "src/Parakeet.Net.Application/"]
COPY ["src/Parakeet.Net.Domain/Parakeet.Net.Domain.csproj", "src/Parakeet.Net.Domain/"]
COPY ["src/Parakeet.Net.Domain.Shared/Parakeet.Net.Domain.Shared.csproj", "src/Parakeet.Net.Domain.Shared/"]
COPY ["src/Parakeet.Net.Application.Contracts/Parakeet.Net.Application.Contracts.csproj", "src/Parakeet.Net.Application.Contracts/"]
COPY ["src/Parakeet.Net.HttpApi/Parakeet.Net.HttpApi.csproj", "src/Parakeet.Net.HttpApi/"]
COPY ["src/Parakeet.Net.EntityFrameworkCore/Parakeet.Net.EntityFrameworkCore.csproj", "src/Parakeet.Net.EntityFrameworkCore/"]
RUN dotnet restore "./src/Parakeet.Net.Web/Parakeet.Net.Web.csproj"
COPY . .
WORKDIR "/src/src/Parakeet.Net.Web"
RUN dotnet build "./Parakeet.Net.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Parakeet.Net.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parakeet.Net.Web.dll"]