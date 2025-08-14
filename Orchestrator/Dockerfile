FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Orchestrator.csproj", "Orchestrator/"]
RUN dotnet restore "Orchestrator/Orchestrator.csproj"

COPY . Orchestrator/.
RUN dotnet publish -c Release -o /app Orchestrator/Orchestrator.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Orchestrator.dll"]