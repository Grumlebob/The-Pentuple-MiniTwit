# Use the official ASP.NET Core runtime image as a base.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK image for building the application.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project file(s) and restore dependencies.
COPY ["MiniTwit.Api/MiniTwit.Api.csproj", "MiniTwit.Api/"]
RUN dotnet restore "MiniTwit.Api/MiniTwit.Api.csproj"

# Copy the remaining source code.
COPY . .
WORKDIR /src/MiniTwit.Api
RUN dotnet build "MiniTwit.Api.csproj" -c Release -o /app/build

# Publish the application.
FROM build AS publish
RUN dotnet publish "MiniTwit.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy the published output and the entrypoint script.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the entrypoint script into the image root (/app)
COPY MiniTwit.Api/entrypoint.sh /app/entrypoint.sh

# Install dos2unix, convert line endings, and set executable permissions
RUN apt-get update && \
    apt-get install --no-install-recommends -y dos2unix=7.4.3-1 && \
    rm -rf /var/lib/apt/lists/*

RUN dos2unix /app/entrypoint.sh && chmod +x /app/entrypoint.sh

# Set the entry point for the container.
ENTRYPOINT ["/app/entrypoint.sh"]
