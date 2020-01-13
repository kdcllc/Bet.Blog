# docker build --rm -f "./src/Bet.Blog.WebApp/Dockerfile" -t kdcll/betblog:v1 .


ARG PROJECT_PATH="./src/Bet.Blog.WebApp/Bet.Blog.WebApp.csproj"

FROM betblogbase:v1 as builder

RUN dotnet publish "./src/Bet.Blog.WebApp/Bet.Blog.WebApp.csproj" -c Release -o /app

# App image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS final
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "Bet.Blog.WebApp.dll"]
