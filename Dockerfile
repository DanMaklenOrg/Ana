FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY . .
RUN ls
RUN dotnet publish Service -c Release -o bin

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/bin .
ENTRYPOINT ["dotnet", "Service.dll"]
