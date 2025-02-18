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
WORKDIR "/src/MiniTwit.Api"
RUN dotnet build "MiniTwit.Api.csproj" -c Release -o /app/build

# Publish the application.
FROM build AS publish
RUN dotnet publish "MiniTwit.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image: copy the published output from the build stage.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point for the container.
ENTRYPOINT ["dotnet", "MiniTwit.Api.dll"]