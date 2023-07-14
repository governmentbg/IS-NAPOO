#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ISNAPOO.WebSystem/ISNAPOO.WebSystem.csproj", "ISNAPOO.WebSystem/"]
RUN dotnet restore "ISNAPOO.WebSystem/ISNAPOO.WebSystem.csproj"
COPY . .
WORKDIR "/src/ISNAPOO.WebSystem"
RUN dotnet build "ISNAPOO.WebSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ISNAPOO.WebSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ISNAPOO.WebSystem.dll"]