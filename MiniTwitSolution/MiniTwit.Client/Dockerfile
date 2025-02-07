# Build stage: use the .NET SDK image.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the client project file and restore.
COPY ["MiniTwit.Client/MiniTwit.Client.csproj", "MiniTwit.Client/"]
RUN dotnet restore "MiniTwit.Client/MiniTwit.Client.csproj"

# Copy the remaining files and publish the project.
COPY . .
WORKDIR "/src/MiniTwit.Client"
RUN dotnet publish -c Release -o /app/publish

# Final stage: use Nginx to serve the static files.
FROM nginx:alpine AS final
# Copy the published wwwroot (Blazor's static assets) to Nginx's default folder.
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
