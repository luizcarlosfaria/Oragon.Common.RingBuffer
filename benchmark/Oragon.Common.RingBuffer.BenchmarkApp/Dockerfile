#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS base
RUN dotnet tool install -g BenchmarkDotNet.Tool
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["./benchmark/Oragon.Common.RingBuffer.BenchmarkApp/Oragon.Common.RingBuffer.BenchmarkApp.csproj", "benchmark/Oragon.Common.RingBuffer.BenchmarkApp/"]
COPY ["./src/Oragon.Common.RingBuffer/Oragon.Common.RingBuffer.csproj", "./src/Oragon.Common.RingBuffer/"]
RUN dotnet restore "benchmark/Oragon.Common.RingBuffer.BenchmarkApp/Oragon.Common.RingBuffer.BenchmarkApp.csproj"
COPY . .
WORKDIR "/src/benchmark/Oragon.Common.RingBuffer.BenchmarkApp"
RUN dotnet build "Oragon.Common.RingBuffer.BenchmarkApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oragon.Common.RingBuffer.BenchmarkApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Oragon.Common.RingBuffer.BenchmarkApp.dll"]
