#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WonderlandChip.WebAPI/WonderlandChip.WebAPI.csproj", "WonderlandChip.WebAPI/"]
COPY ["WonderlandChip.Database/WonderlandChip.Database.csproj", "WonderlandChip.Database/"]
RUN dotnet restore "WonderlandChip.WebAPI/WonderlandChip.WebAPI.csproj"
COPY . .
WORKDIR "/src/WonderlandChip.WebAPI"
RUN dotnet build "WonderlandChip.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WonderlandChip.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WonderlandChip.WebAPI.dll"]