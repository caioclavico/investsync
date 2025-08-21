# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY Api/Api.csproj ./Api/
COPY Backend/Shared/Shared.csproj ./Backend/Shared/

# Restore dependencies
RUN dotnet restore ./Api/Api.csproj

# Copy the source code
COPY Api/ ./Api/
COPY Backend/Shared/ ./Backend/Shared/

# Build the application
RUN dotnet publish ./Api/Api.csproj -c Release -o /app/publish

# Use the official .NET 9 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Create .env file with default values
RUN echo 'JWT_KEY=your-super-secret-jwt-key-here-make-it-at-least-32-characters-long' > .env && \
    echo 'JWT_ISSUER=InvestSync' >> .env

# Expose the port
EXPOSE 5000

# Set the entry point
ENTRYPOINT ["dotnet", "Api.dll"]
