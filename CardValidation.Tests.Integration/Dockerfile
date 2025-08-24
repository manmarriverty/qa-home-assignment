FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /tests

COPY . .
RUN dotnet restore

CMD dotnet test CardValidation.Tests.Integration/CardValidation.Tests.Integration.csproj \
    --logger "trx;LogFileName=integration-tests.trx" \
    --results-directory /tests/CardValidation.Tests.Integration/TestResults
