FROM microsoft/dotnet:2.1.302-sdk AS base
WORKDIR /app
EXPOSE ${PORT:-80}

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
CMD ASPNETCORE_URLS=http://+:${PORT:-80} dotnet HackneyRepairs.dll