#------------------------------------------------------------------------------------------------------------------------
#
# BUILD: docker build --rm -f "./src/Bet.Blog.WebApp/Dockerfile" -t kdcllc/betblog:v1 .
#
# REBUILD: docker build --no-cache --rm -f "./src/Bet.Blog.WebApp/Dockerfile" -t kdcllc/betblog:v1 .
#
# RUN: docker run -d -p 4002:80 -e ASPNETCORE_ENVIRONMENT=Development -e MSI_ENDPOINT=$Env:MSI_ENDPOINT -e MSI_SECRET=$Env:MSI_SECRET kdcllc/betblog:v1
#
#------------------------------------------------------------------------------------------------------------------------

FROM kdcllc/betblog:base as builder
RUN dotnet publish "./src/Bet.Blog.WebApp/Bet.Blog.WebApp.csproj" -r linux-musl-x64 -o out --self-contained true /p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine AS runtime
WORKDIR /app

COPY --from=builder /app/out ./
ENTRYPOINT ["./Bet.Blog.WebApp"]


