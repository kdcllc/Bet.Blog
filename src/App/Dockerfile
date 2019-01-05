FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["DabarBlog/DabarBlog.csproj", "DabarBlog/"]
RUN dotnet restore "DabarBlog/DabarBlog.csproj"
COPY . .
WORKDIR "/src/DabarBlog"
RUN dotnet build "DabarBlog.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DabarBlog.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DabarBlog.dll"]