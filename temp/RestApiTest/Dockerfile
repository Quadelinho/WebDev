FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["RestApiTest/RestApiTest.csproj", "RestApiTest/"]
RUN dotnet restore "RestApiTest/RestApiTest.csproj"
COPY . .
WORKDIR "/src/RestApiTest"
RUN dotnet build "RestApiTest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RestApiTest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RestApiTest.dll"]