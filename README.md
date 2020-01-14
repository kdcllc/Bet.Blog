# Bet.Blog Project

The goal of this project is to develop a blogging platform for Biblical oriented bloggers.

- Support for different version of the Bible quotations
- Support for Hebrew and Greek

## Technology stack

- ASP.NET Core
- Docker
- Kubenetes
- Azure Key Vault - stores all of the sensitive information.

## Build Docker

```bash
    docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml"  up -d --build

    # https://docs.microsoft.com/en-us/visualstudio/containers/container-build?view=vs-2019#msbuild
    msbuild /p:SolutionPath=Bet.Blog.sln /p:Configuration=Release docker-compose.dcproj
```

## [Dotnet EF global tooling](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)

```bash
    dotnet tool install --global dotnet-ef
```


## Using Visual Studio.NET Containers

1. Start `appauthentication`

2. Build base image first

```bash
    docker build --rm -f "Dockerfile.Base" -t betblogbase:v1 --build-arg NUGET_RESTORE="-v=q" .
```

3. Open Visual Studio.NET

If `betblogbase` image is not present locally the Container tooling will fail.
