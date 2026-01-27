# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["InfotecsApi/InfotecsApi.csproj", "InfotecsApi/"]
RUN dotnet restore "InfotecsApi/InfotecsApi.csproj"
COPY . .
WORKDIR "/src/InfotecsApi"
RUN dotnet build "InfotecsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InfotecsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InfotecsApi.dll"]