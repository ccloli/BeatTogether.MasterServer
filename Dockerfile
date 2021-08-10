FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Restore the project
COPY . ./
RUN dotnet restore

# Build the project
RUN ARCH= && \
    case "$(dpkg --print-architecture)" in \
        amd64) ARCH='x64';; \
        i386) ARCH='x86';; \
        arm64) ARCH='arm64';; \
        armhf) ARCH='arm';; \
        armel) ARCH='arm';; \
        *) echo "unsupported architecture"; exit 1 ;; \
    esac && \
    dotnet publish BeatTogether.MasterServer -c Release -p:PublishReadyToRun=true -r "linux-$ARCH" -o out

# Run the application
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "/app/BeatTogether.MasterServer.dll"]
