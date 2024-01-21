FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.fsproj ./
RUN dotnet restore

# copy and publish app and libraries
COPY . ./
RUN dotnet publish -c Release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-arm32v7

# Install cultures (alpine doesn't come with them by default)
RUN apk add --no-cache icu-libs tzdata bash
# Disable the invariant mode for the base image
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
# setting timezone
ENV TZ=Europe/Copenhagen

WORKDIR /app
COPY --from=build /app ./
#ADD data data
ENTRYPOINT ["dotnet", "fping.dll"]