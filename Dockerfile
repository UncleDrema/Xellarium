FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Скопировать файлы решений и проектов
COPY . .

RUN dotnet restore

FROM build AS final

WORKDIR /app

# Команда по умолчанию для выполнения тестов
ENTRYPOINT ["dotnet", "test"]
