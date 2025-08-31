ARG DOTNET_VERSION=8.0
ARG ASPNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${ASPNET_VERSION} AS base
WORKDIR /app
# for http
EXPOSE 5219
# for https
EXPOSE 7045

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

#start-up
COPY ServerApp/ServerApp.csproj ServerApp/
COPY ServerApp.BusinessLogic/ServerApp.BusinessLogic.csproj ServerApp.BusinessLogic/
COPY ServerApp.DataAccess/ServerApp.DataAccess.csproj ServerApp.DataAccess/

RUN dotnet restore ServerApp/ServerApp.csproj 

COPY . .

#build
WORKDIR /src/ServerApp
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServerApp.dll"]

