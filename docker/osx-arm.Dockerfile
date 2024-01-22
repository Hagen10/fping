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

FROM mcr.microsoft.com/dotnet/aspnet:7.0.15-alpine3.18-arm64v8 AS runtime

# install necessary network utilities
RUN apk --no-cache add iputils

# Set the working directory in the container
WORKDIR /app

# Copy the published output from the build image
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "fping.dll"]
