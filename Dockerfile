FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "./"]
COPY ["CardValidation.Web/CardValidation.Web.csproj", "CardValidation.Web/"]
COPY ["CardValidation.Core/CardValidation.Core.csproj", "CardValidation.Core/"]
RUN dotnet restore "CardValidation.Web/CardValidation.Web.csproj"
COPY . .
WORKDIR "/src/CardValidation.Web"
RUN dotnet build "CardValidation.Web.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /src
COPY ["CardValidation.Core.Tests/CardValidation.Core.Tests.csproj", "CardValidation.Core.Tests/"]
COPY ["CardValidation.IntegrationTests/CardValidation.IntegrationTests.csproj", "CardValidation.IntegrationTests/"]
RUN dotnet restore
COPY . .
RUN dotnet test --no-restore --verbosity minimal

FROM build AS publish
RUN dotnet publish "CardValidation.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CardValidation.Web.dll"]

