FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app
EXPOSE 5050
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:5050

ARG backend_url=host.docker.internal
ENV BACKEND_URL=$backend_url 


# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["WebGui/WebGui.csproj", "WebGui/"]
COPY ["Library/Library.csproj", "Library/"]
RUN dotnet restore "WebGui/WebGui.csproj"
RUN dotnet restore "Library/Library.csproj"
COPY . .
WORKDIR "/src/WebGui"
RUN dotnet build "WebGui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebGui.csproj" -c Release -o /app/publish

RUN chmod -R 765 /app/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebGui.dll"]
