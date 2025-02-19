# --------------------------
# 1) Build stage
# --------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the Client and Shared .csproj files for restore
COPY MiniTwit.Client/MiniTwit.Client.csproj ./MiniTwit.Client/
COPY MiniTwit.Shared/MiniTwit.Shared.csproj ./MiniTwit.Shared/

# Restore the client project
RUN dotnet restore "MiniTwit.Client/MiniTwit.Client.csproj"

# Copy the rest of the solution
COPY . .

# Switch working directory to the Client project
WORKDIR "/src/MiniTwit.Client"

# Publish the Blazor WASM app
RUN dotnet publish -c Release -o /app/publish

# --------------------------
# 2) Final stage: Nginx
# --------------------------
FROM nginx:alpine AS final

# Remove the stock default.conf so it doesn't conflict with ours
RUN rm -f /etc/nginx/conf.d/*

# Copy published Blazor files into Nginx's html folder
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Copy your custom fallback config to handle client-side routes
COPY MiniTwit.Client/default.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# Start Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]