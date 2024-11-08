# Build Context to this file is ./
#
# Build the image with the following command:
# log4TCAdsRouterversion=$(sed -n 's|.*<Version>\([^<]*\)</Version>.*|\1|p' Directory.Build.props) && \
# podman build \
#   --build-arg BUILD_VERSION="$log4TCAdsRouterversion" \
#   --build-arg BUILD_DATE="$(date -u +'%Y-%m-%dT%H:%M:%SZ')" \
#   --build-arg SOURCECOMMITID="$(git rev-parse HEAD)" \
#   -t log4tc-adsrouter:$log4TCAdsRouterversion-beta .

# Base Image stage
########################
FROM mcr.microsoft.com/dotnet/runtime:8.0-bookworm-slim AS base
ARG BUILD_VERSION
ARG BUILD_DATE
ARG SOURCECOMMITID
WORKDIR /app
# The ADSRouter used TcpPort (48898 or 0xBF02 by default)
EXPOSE 48898

# The OCI Spec Labels https://github.com/opencontainers/image-spec/blob/main/annotations.md
LABEL org.opencontainers.image.version=$BUILD_VERSION
LABEL org.opencontainers.image.created=$BUILD_DATE
LABEL org.opencontainers.image.title=log4tc-adsRouter
LABEL org.opencontainers.image.authors=bqstony
LABEL org.opencontainers.image.source=https://github.com/mbc-engineering/log4TC
LABEL org.opencontainers.image.revision=$SOURCECOMMITID
LABEL org.opencontainers.image.base.name=mcr.microsoft.com/dotnet/runtime:8.0-bookworm-slim
LABEL org.opencontainers.image.vendor="mbc-engineering"

# Stages for building the app
########################
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS publish
WORKDIR /src
COPY src/. .
RUN dotnet publish --framework net8.0 --configuration Release --self-contained false --runtime linux-x64 --output /app/publish

# Stage for pack the binary to deploymnd container
########################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY src/appsettingsSample.json ./appsettings.json

ENTRYPOINT ["dotnet", "AdsRouterConsoleApp.dll"]
