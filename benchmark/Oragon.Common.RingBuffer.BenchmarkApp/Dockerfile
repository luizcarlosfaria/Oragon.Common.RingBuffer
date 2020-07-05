#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["./benchmark/Oragon.Common.RingBuffer.ConsoleTestApp/Oragon.Common.RingBuffer.ConsoleTestApp.csproj", "benchmark/Oragon.Common.RingBuffer.ConsoleTestApp/"]
COPY ["./src/Oragon.Common.RingBuffer/Oragon.Common.RingBuffer.csproj", "./src/Oragon.Common.RingBuffer/"]
RUN dotnet restore "benchmark/Oragon.Common.RingBuffer.ConsoleTestApp/Oragon.Common.RingBuffer.ConsoleTestApp.csproj"
COPY . .
WORKDIR "/src/benchmark/Oragon.Common.RingBuffer.ConsoleTestApp"
RUN dotnet build "Oragon.Common.RingBuffer.ConsoleTestApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oragon.Common.RingBuffer.ConsoleTestApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Oragon.Common.RingBuffer.ConsoleTestApp.dll"]
