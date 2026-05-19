FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution structure
COPY ProudSmileDent/ ProudSmileDent/

WORKDIR /src/ProudSmileDent

RUN dotnet restore ProudSmileDent.csproj
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "ProudSmileDent.dll"]