FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS final
ENV INSTRUMENTATION_KEY="insert-key-here"
ENV DATA_PATH="/data"
ENV SCHEDULE-CRAWL-INTERVAL="5"
EXPOSE 80
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "megaphone.feeds.dll"]