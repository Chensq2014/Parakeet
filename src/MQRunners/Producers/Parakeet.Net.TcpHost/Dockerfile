#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/MQRunners/Producers/Parakeet.Net.TcpHost/Parakeet.Net.TcpHost.csproj", "src/MQRunners/Producers/Parakeet.Net.TcpHost/"]
RUN dotnet restore "src/MQRunners/Producers/Parakeet.Net.TcpHost/Parakeet.Net.TcpHost.csproj"
COPY . .
WORKDIR "/src/src/MQRunners/Producers/Parakeet.Net.TcpHost"
RUN dotnet build "Parakeet.Net.TcpHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Parakeet.Net.TcpHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parakeet.Net.TcpHost.dll"]