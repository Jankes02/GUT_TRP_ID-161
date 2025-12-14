FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MonolithicApp/MonolithicApp.csproj", "MonolithicApp/"]
RUN dotnet restore "MonolithicApp/MonolithicApp.csproj"
COPY . .
WORKDIR "/src/MonolithicApp"
RUN dotnet publish "MonolithicApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MonolithicApp.dll"]
