# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY CardValidation.Core/*.csproj CardValidation.Core/
COPY CardValidation.Web/*.csproj CardValidation.Web/
COPY CardValidation.Core.Tests/*.csproj CardValidation.Core.Tests/
COPY CardValidation.Web.Tests/*.csproj CardValidation.Web.Tests/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish CardValidation.Web -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "CardValidation.Web.dll"]

