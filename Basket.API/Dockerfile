FROM microsoft/dotnet:2.1.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 60676
EXPOSE 44311

FROM microsoft/dotnet:2.1.301-sdk AS build
WORKDIR /src
COPY Basket.API/Basket.API.csproj Basket.API/
RUN dotnet restore Basket.API/Basket.API.csproj
COPY . .
WORKDIR /src/Basket.API
RUN dotnet build Basket.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Basket.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Basket.API.dll"]
