FROM microsoft/dotnet:2.1.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54224
EXPOSE 44322

FROM microsoft/dotnet:2.1.1-sdk AS build
WORKDIR /src
COPY ClientApp/ClientApp.csproj ClientApp/
RUN dotnet restore ClientApp/ClientApp.csproj
COPY . .
WORKDIR /src/ClientApp
RUN dotnet build ClientApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ClientApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ClientApp.dll"]
