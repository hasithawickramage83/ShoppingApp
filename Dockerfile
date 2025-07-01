# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# ✅ Expose port 80 for incoming traffic
EXPOSE 80

# Stage 2: Build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Final image
FROM base AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# ✅ Make sure app listens on all IPs inside container
ENV ASPNETCORE_URLS=http://+:80

# Start the application
ENTRYPOINT ["dotnet", "ShoppingApp.dll"]
