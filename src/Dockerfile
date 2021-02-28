FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS final
EXPOSE 80
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "megaphone.crawler.dll"]