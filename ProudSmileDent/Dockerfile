FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ProudSmileDent/ProudSmileDent.csproj ./ProudSmileDent/

RUN dotnet restore ./ProudSmileDent/ProudSmileDent.csproj

COPY ProudSmileDent/ ./ProudSmileDent/

WORKDIR /src/ProudSmileDent
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "ProudSmileDent.dll"]