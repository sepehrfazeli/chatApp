# Use .NET 9 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

# Use .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["chatApp.csproj", "./"]
RUN dotnet restore "chatApp.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "chatApp.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "chatApp.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for SQLite database
RUN mkdir -p /app/data

# Set environment variable for database path
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/chat.db"

ENTRYPOINT ["dotnet", "chatApp.dll"]