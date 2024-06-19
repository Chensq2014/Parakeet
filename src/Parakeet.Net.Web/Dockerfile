#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["packages/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic/Directory.Packages.props", "packages/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic/"]
COPY ["packages/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic/Directory.Build.props", "packages/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic/"]
COPY ["src/Parakeet.Net.Web/Parakeet.Net.Web.csproj", "src/Parakeet.Net.Web/"]
COPY ["packages/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.csproj", "packages/Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic/"]
COPY ["src/Common/Core/Common.Nacos/Common.Nacos.csproj", "src/Common/Core/Common.Nacos/"]
COPY ["src/Common/Common.Shared/Common.Shared.csproj", "src/Common/Common.Shared/"]
COPY ["src/Parakeet.Net.Application/Parakeet.Net.Application.csproj", "src/Parakeet.Net.Application/"]
COPY ["src/Common/Common/Common.csproj", "src/Common/Common/"]
COPY ["src/Common/Common.Entities/Common.Entities.csproj", "src/Common/Common.Entities/"]
COPY ["src/Common/Core/Common.RabbitMQModule/Common.RabbitMQModule.csproj", "src/Common/Core/Common.RabbitMQModule/"]
COPY ["src/Grpc/GrpcClients/Parakeet.Net.RoClient/Parakeet.Net.ROClient.csproj", "src/Grpc/GrpcClients/Parakeet.Net.RoClient/"]
COPY ["src/Parakeet.Net.Domain/Parakeet.Net.Domain.csproj", "src/Parakeet.Net.Domain/"]
COPY ["src/Common/Core/Common.CacheMudule/Common.CacheMudule.csproj", "src/Common/Core/Common.CacheMudule/"]
COPY ["src/Parakeet.Net.Domain.Shared/Parakeet.Net.Domain.Shared.csproj", "src/Parakeet.Net.Domain.Shared/"]
COPY ["src/Parakeet.Net.Application.Contracts/Parakeet.Net.Application.Contracts.csproj", "src/Parakeet.Net.Application.Contracts/"]
COPY ["src/Parakeet.Net.MultiTenancy/Parakeet.Net.MultiTenancy.csproj", "src/Parakeet.Net.MultiTenancy/"]
COPY ["src/Parakeet.Net.EntityFrameworkCore/Parakeet.Net.EntityFrameworkCore.csproj", "src/Parakeet.Net.EntityFrameworkCore/"]
COPY ["src/Parakeet.Net.HttpApi.Client/Parakeet.Net.HttpApi.Client.csproj", "src/Parakeet.Net.HttpApi.Client/"]
COPY ["src/Parakeet.Net.HttpApi/Parakeet.Net.HttpApi.csproj", "src/Parakeet.Net.HttpApi/"]
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