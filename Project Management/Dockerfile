#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
#EXPOSE 5000
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Project Management/Project Management.csproj", "Project Management/"]
RUN dotnet restore "Project Management/Project Management.csproj"

COPY . .
WORKDIR "/src/Project Management"

RUN dotnet build "Project Management.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project Management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Project Management/Https/midouz.online.pfx", "/https/midouz.online.pfx"]
COPY ["Project Management/Https/midouz_online.crt","/usr/local/share/ca-certificates/midouz_online.crt"]
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "Project Management.dll"]
