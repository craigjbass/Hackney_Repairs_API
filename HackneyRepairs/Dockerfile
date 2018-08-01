FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE ${PORT:-80}

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY HackbeyRepairs.sln ./
COPY HackbeyRepairs/HackneyRepairs.csproj HackbeyRepairs/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/HackbeyRepairs
RUN dotnet build HackneyRepairs.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish HackneyRepairs.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ASPNETCORE_URLS=http://+:${PORT:-80} dotnet HackneyRepairs.dll
