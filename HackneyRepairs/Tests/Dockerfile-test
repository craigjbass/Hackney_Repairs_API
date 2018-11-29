FROM microsoft/dotnet:2.1.302-sdk AS base
WORKDIR /src
COPY HackneyRepairs.sln ./
COPY HackneyRepairs/HackneyRepairs.csproj HackneyRepairs/
RUN dotnet restore HackneyRepairs/HackneyRepairs.csproj -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/HackneyRepairs
RUN dotnet build HackneyRepairs.csproj -c Release -o /app