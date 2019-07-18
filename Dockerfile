FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS builder
WORKDIR /build
COPY rabbithole.csproj .
RUN dotnet restore
COPY * ./
RUN dotnet publish -c Release --no-restore -o out

FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
ARG USER=rabbithole
ARG UID=1234
RUN useradd -u ${UID} ${USER}
RUN chown ${USER} /app
USER ${USER}
COPY --from=builder /build/out/* ./
COPY appsettings.default.json ./
CMD dotnet rabbithole.dll