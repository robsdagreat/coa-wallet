# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["YourProjectName.csproj", "./"]

RUN dotnet restore "coa-wallet.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet publish "coa-wallet.csproj" -c Release -o /app/publish

# Set up runtime environment
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "coa-wallet.dll"]

