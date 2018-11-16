FROM microsoft/dotnet:2.1.500-sdk AS base
WORKDIR /app
EXPOSE ${PORT:-80}
RUN apt-get update && apt-get install -y samba smbclient cifs-utils

FROM microsoft/dotnet:2.1.500-sdk AS build
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

ENV CORECLR_ENABLE_PROFILING=1 \
  CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
  CORECLR_NEWRELIC_HOME=./newrelic \
  CORECLR_PROFILER_PATH=./newrelic/libNewRelicProfiler.so \
  NEW_RELIC_LICENSE_KEY="${NEW_RELIC_LICENSE_KEY}" \
  NEW_RELIC_APP_NAME="${NEW_RELIC_APP_NAME}"

COPY --from=publish /app .
COPY newrelic ./newrelic

EXPOSE ${PORT:-80}
CMD ASPNETCORE_URLS=http://+:${PORT:-80} dotnet HackneyRepairs.dll
