ARG sdkImage=kdcllc/dotnet
ARG sdkTag=2.2-sdk
ARG runtimeImage=microsoft/dotnet
ARG runtimeTag=2.2-aspnetcore-runtime
ARG coreversion=netcore22

# Build image
FROM ${sdkImage}:${sdkTag} as builder
RUN npm -v

RUN dotnet publish "./src/App/App.csproj" -c Release -p:NetCoreVersion=${coreversion} -o /app

# App image
FROM ${runtimeImage}:${runtimeTag} AS final
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "App.dll"]
