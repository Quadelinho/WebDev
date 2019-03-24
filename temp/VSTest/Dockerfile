FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["VSTest/VSTest.csproj", "VSTest/"]
RUN dotnet restore "VSTest/VSTest.csproj"
COPY . .
WORKDIR "/src/VSTest"
RUN dotnet build "VSTest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VSTest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VSTest.dll"]