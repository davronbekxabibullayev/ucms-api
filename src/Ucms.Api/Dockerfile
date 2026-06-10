FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

#Copy resources to docket layer
COPY . .

#Build artifact
RUN cd ./src/Services/Stock/Ucms.Stock.Api && \
    dotnet restore && \
    dotnet build "Ucms.Stock.Api.csproj" -c Release -o /app/build && \
    dotnet publish "Ucms.Stock.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS app

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80/tcp
EXPOSE 443/tcp

ENTRYPOINT ["dotnet", "Ucms.Stock.Api.dll"]
