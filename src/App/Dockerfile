FROM kdcllc/dotnet:3.0-sdk-buster as builder

RUN dotnet publish "./src/App/App.csproj" -c Release -o /app

# App image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS final
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "App.dll"]
