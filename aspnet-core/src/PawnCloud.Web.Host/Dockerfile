FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/PawnCloud.Web.Host/PawnCloud.Web.Host.csproj", "src/PawnCloud.Web.Host/"]
COPY ["src/PawnCloud.Web.Core/PawnCloud.Web.Core.csproj", "src/PawnCloud.Web.Core/"]
COPY ["src/PawnCloud.Application/PawnCloud.Application.csproj", "src/PawnCloud.Application/"]
COPY ["src/PawnCloud.Core/PawnCloud.Core.csproj", "src/PawnCloud.Core/"]
COPY ["src/PawnCloud.EntityFrameworkCore/PawnCloud.EntityFrameworkCore.csproj", "src/PawnCloud.EntityFrameworkCore/"]
WORKDIR "/src/src/PawnCloud.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/PawnCloud.Web.Host", "src/PawnCloud.Web.Host"]
COPY ["src/PawnCloud.Web.Core", "src/PawnCloud.Web.Core"]
COPY ["src/PawnCloud.Application", "src/PawnCloud.Application"]
COPY ["src/PawnCloud.Core", "src/PawnCloud.Core"]
COPY ["src/PawnCloud.EntityFrameworkCore", "src/PawnCloud.EntityFrameworkCore"]
WORKDIR "/src/src/PawnCloud.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "PawnCloud.Web.Host.dll"]
