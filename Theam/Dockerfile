FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 52574
EXPOSE 44304

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Theam/Theam.API.csproj Theam/
RUN dotnet restore Theam/Theam.API.csproj
COPY . .
WORKDIR /src/Theam
RUN dotnet build Theam.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Theam.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Theam.API.dll"]
