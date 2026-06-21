# Multi-stage build for Cloud Run. SDK pinned to .NET 9 to match global.json so the
# Blazor WASM client + API publish exactly as they do locally; runtime image is the slim
# ASP.NET Core 9 base. The app reads PORT and binds 0.0.0.0 (Cloud Run injects PORT).

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore against the full solution graph first for better layer caching.
COPY global.json ./
COPY TuringsLastCipher.sln ./
COPY src/ ./src/
COPY content/ ./content/
RUN dotnet restore src/TuringsLastCipher.Api/TuringsLastCipher.Api.csproj

# Publish the API; it pulls in the Core lib and the hosted WASM client, and copies
# content/scenes/story.json beside the binary (see Api.csproj).
RUN dotnet publish src/TuringsLastCipher.Api/TuringsLastCipher.Api.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
# Cloud Run sets PORT; Program.cs honours it. 8080 is the documented default.
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "TuringsLastCipher.Api.dll"]
