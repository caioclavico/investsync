# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY Worker/Worker.csproj ./Worker/
COPY Shared/Shared.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Worker/Worker.csproj

# Copy the source code
COPY Worker/ ./Worker/
COPY Shared/ ./Shared/

# Build the application
RUN dotnet publish ./Worker/Worker.csproj -c Release -o /app/publish

# Use the official .NET 9 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose the port (if needed)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Worker.dll"]
