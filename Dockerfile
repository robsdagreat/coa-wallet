# Use the .NET 8.0 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["coa-wallet.csproj", "./"]
RUN dotnet restore "coa-wallet.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet publish "coa-wallet.csproj" -c Release -o /app/publish

# Use the .NET 8.0 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set up runtime environment
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "coa-wallet.dll"]
