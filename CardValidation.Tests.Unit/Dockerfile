FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /tests

COPY . .
RUN dotnet restore

CMD dotnet test CardValidation.Tests.Unit/CardValidation.Tests.Unit.csproj \
    --logger "trx;LogFileName=unit-tests.trx" \
    --results-directory /tests/CardValidation.Tests.Unit/TestResults
