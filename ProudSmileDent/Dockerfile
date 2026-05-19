FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

WORKDIR /src/ProudSmileDent
RUN dotnet restore ProudSmileDent.csproj
RUN dotnet publish ProudSmileDent.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProudSmileDent.dll"]