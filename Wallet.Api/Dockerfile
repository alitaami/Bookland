# Use the official SDK image as a parent image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base

# Define build arguments and environment variables
ARG BUILD_CONFIGURATION=Release
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

# Expose port 80 for the application to listen on
EXPOSE 80

# Set the working directory to /src
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

# Copy the rest of the application source code
COPY . .

# Set the working directory to the Wallet.Api project
WORKDIR "/src/Wallet.Api"

# Build the application in Release mode and output to /app/build
RUN dotnet build "Wallet.Api.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

# Specify the entry point and command to run the application
ENTRYPOINT ["dotnet", "/app/build/Wallet.Api.dll"]
CMD ["--server.urls", "http://0.0.0.0:5000"]
 

