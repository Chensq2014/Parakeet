#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/Grpc/GrpcServers/Reverse/Parakeet.Net.ROServer/Parakeet.Net.ROServer.csproj", "src/Grpc/GrpcServers/Reverse/Parakeet.Net.ROServer/"]
RUN dotnet restore "src/Grpc/GrpcServers/Reverse/Parakeet.Net.ROServer/Parakeet.Net.ROServer.csproj"
COPY . .
WORKDIR "/src/src/Grpc/GrpcServers/Reverse/Parakeet.Net.ROServer"
RUN dotnet build "Parakeet.Net.ROServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Parakeet.Net.ROServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parakeet.Net.ROServer.dll"]