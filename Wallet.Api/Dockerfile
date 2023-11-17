# Stage 1: Build and restore
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project files
COPY ["Wallet.Api/Wallet.Api.csproj", "Wallet.Api/"]
COPY ["Wallet.Webframework/Wallet.Webframework.csproj", "Wallet.Webframework/"]
COPY ["Wallet.Infrastructure/Wallet.Infrastructure.csproj", "Wallet.Infrastructure/"]
COPY ["Wallet.Application/Wallet.Application.csproj", "Wallet.Application/"]
COPY ["Wallet.Core/Wallet.Core.csproj", "Wallet.Core/"]
COPY ["Wallet.Common/Wallet.Common.csproj", "Wallet.Common/"]

# Adjust the paths and update the restore commands
RUN dotnet restore "Wallet.Api/Wallet.Api.csproj"
RUN dotnet restore "Wallet.Webframework/Wallet.Webframework.csproj"
RUN dotnet restore "Wallet.Infrastructure/Wallet.Infrastructure.csproj"
RUN dotnet restore "Wallet.Application/Wallet.Application.csproj"
RUN dotnet restore "Wallet.Core/Wallet.Core.csproj"
RUN dotnet restore "Wallet.Common/Wallet.Common.csproj"

# Copy the entire project files
COPY . .

WORKDIR "/src/Wallet.Api"
RUN dotnet build "Wallet.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Wallet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 5000

# Copy the published artifacts from the 'publish' stage
COPY --from=publish /app/publish .

# Specify the exact command to run your application
ENTRYPOINT ["dotnet", "Wallet.Api.dll"]
