# syntax=docker/dockerfile:1.7

# Один Dockerfile собирает оба .NET-приложения. Проект выбирают build args.
ARG PROJECT=LingvoGameOsWeb
ARG CSPROJ=LingvoGameOsWeb/LingvoGameOs.csproj

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT
ARG CSPROJ

WORKDIR /src
COPY LingvoGameOs.sln ./
COPY LingvoGameOs.Db/LingvoGameOs.Db.csproj LingvoGameOs.Db/
COPY LingvoGameOs.Shared/LingvoGameOs.Shared.csproj LingvoGameOs.Shared/
COPY LingvoGameOsWeb/LingvoGameOs.csproj LingvoGameOsWeb/
COPY LingvoGameOsWebApi/LingvoGameOsWebApi.csproj LingvoGameOsWebApi/

# Web и API собираются параллельно. Разные cache id не дают двум restore
# одновременно распаковывать одни и те же NuGet-пакеты.
RUN --mount=type=cache,id=nuget-${PROJECT},target=/root/.nuget/packages \
    dotnet restore "${CSPROJ}"

COPY . .
RUN --mount=type=cache,id=nuget-${PROJECT},target=/root/.nuget/packages \
    dotnet publish "/src/${CSPROJ}" \
    -c Release \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG CSPROJ

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0

COPY --from=build --chown=app:app /app/publish /app

# Имя DLL совпадает с именем csproj, а не обязательно с именем каталога.
RUN printf '#!/bin/sh\nexec dotnet %s.dll\n' "$(basename "${CSPROJ}" .csproj)" > /entrypoint.sh \
    && chmod 0555 /entrypoint.sh

WORKDIR /app
USER app

EXPOSE 8080

# В стандартном aspnet:8.0 есть bash; отдельный curl в runtime-образ не ставим.
HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=5 \
    CMD bash -c 'exec 3<>/dev/tcp/127.0.0.1/8080' || exit 1

ENTRYPOINT ["/entrypoint.sh"]
