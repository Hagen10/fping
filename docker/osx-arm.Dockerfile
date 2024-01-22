# Use the official .NET SDK image for ARM64
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory in the container
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.fsproj ./
RUN dotnet restore

# Copy the F# project files to the container
COPY . .
RUN dotnet publish -c Release -o /app

# Build the F# project
# RUN dotnet build -c Release

# # Publish the F# project
# RUN dotnet publish -c Release -o out

# FROM arm64v8/alpine:20231219 AS base

# # Install necessary tools
# RUN apk --no-cache add iputils

# Use the official .NET runtime image for ARM64
# FROM mcr.microsoft.com/dotnet/aspnet:7.0.15-jammy-arm64v8 AS runtime
# FROM mcr.microsoft.com/dotnet/runtime:7.0-jammy-arm64v8 AS runtime

FROM mcr.microsoft.com/dotnet/aspnet:7.0.15-alpine3.18-arm64v8 AS runtime

RUN apk --no-cache add iputils

# Set the working directory in the container
WORKDIR /app

# Copy the published output from the build image
COPY --from=build / ./

# COPY --from=base / ./

# RUN apk --no-cache add iputils

# Run the F# application
# CMD ["./app/fping"]

ENTRYPOINT ["dotnet", "app/fping.dll"]

# ENTRYPOINT ["tail", "-f", "/dev/null"]