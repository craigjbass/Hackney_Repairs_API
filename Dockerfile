FROM microsoft/dotnet:2.1.302-sdk AS base
WORKDIR /app
EXPOSE ${PORT:-80}
RUN apt-get update && apt-get install -y samba smbclient cifs-utils

FROM microsoft/dotnet:2.1.302-sdk AS build
WORKDIR /src
COPY HackneyRepairs.sln ./
COPY HackneyRepairs/HackneyRepairs.csproj HackneyRepairs/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/HackneyRepairs
RUN dotnet build HackneyRepairs.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish HackneyRepairs.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD mount -t cifs ${MOBILE_REPORTS_SERVER_PATH} /mnt -o username=${MOBILE_REPORTS_USER},password=${MOBILE_REPORTS_PASSWORD} && ASPNETCORE_URLS=http://+:${PORT:-80} dotnet HackneyRepairs.dll
