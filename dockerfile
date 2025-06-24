# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CardValidation.sln .
COPY CardValidation.Core/CardValidation.Core.csproj CardValidation.Core/
COPY CardValidation.Tests/CardValidation.Tests.csproj CardValidation.Tests/
COPY CardValidation.Web/CardValidation.Web.csproj CardValidation.Web/

RUN dotnet restore CardValidation.sln
COPY . .

# Build the solution
RUN dotnet build CardValidation.sln -c Release

# Publish the web application
RUN dotnet publish CardValidation.Web/CardValidation.Web.csproj -c Release -o /app/publish

# --- Test stage with verbose output ---
FROM build AS test
WORKDIR /src

RUN mkdir -p /app/test-results

# Run tests with detailed output - you'll see pass/fail in build output
RUN dotnet test CardValidation.Tests/CardValidation.Tests.csproj -v detailed

RUN mkdir -p /app/allure-results && \
    cp -r /src/CardValidation.Tests/allure-results/* /app/allure-results/ 2>/dev/null || true

# --- Interactive test stage (for manual testing) ---
FROM build AS test-interactive
WORKDIR /src
CMD ["dotnet", "test", "CardValidation.Tests/CardValidation.Tests.csproj", "-v", "normal"]

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CardValidation.Web.dll"]