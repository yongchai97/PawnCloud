FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY . .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PawnCloud.Web.Host.dll"]
