# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY Processor/Processor.csproj ./Processor/
COPY Shared/Shared.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Processor/Processor.csproj

# Copy the source code
COPY Processor/ ./Processor/
COPY Shared/ ./Shared/

# Build the application
RUN dotnet publish ./Processor/Processor.csproj -c Release -o /app/publish

# Use the official .NET 9 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose the port (if needed)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Processor.dll"]
