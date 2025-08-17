FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG SERVICE_NAME=TicketOffice.API
WORKDIR /src
COPY ["${SERVICE_NAME}/${SERVICE_NAME}.csproj", "${SERVICE_NAME}/"]
RUN dotnet restore "${SERVICE_NAME}/${SERVICE_NAME}.csproj"

COPY TicketOffice.Contracts TicketOffice.Contracts
COPY ${SERVICE_NAME} ${SERVICE_NAME}

RUN dotnet publish -c Release -o /app ${SERVICE_NAME}/${SERVICE_NAME}.csproj 

FROM base AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "TicketOffice.API.dll"]