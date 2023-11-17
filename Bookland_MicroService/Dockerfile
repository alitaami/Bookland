#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Bookland_MicroService/Bookland_MicroService.csproj", "Bookland_MicroService/"]
RUN dotnet restore "Bookland_MicroService/Bookland_MicroService.csproj"
COPY . .
WORKDIR "/src/Bookland_MicroService"
RUN dotnet build "Bookland_MicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookland_MicroService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookland_MicroService.dll"]