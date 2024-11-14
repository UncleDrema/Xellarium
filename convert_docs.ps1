# Проверка на наличие файла
if (!(Test-Path "./static/documentation.md")) {
    Write-Output "Файл documentation.md не найден."
    exit
}

# Запуск Docker контейнера для конвертации
docker run --rm -v "${PWD}/static:/data" pandoc/core -f markdown -t html -s --metadata title="Xellarium Documentation" -o /data/documentation.html /data/documentation.md

# Проверка результата
if (Test-Path "./static/documentation.html") {
    Write-Output "Конвертация завершена. Файл documentation.html создан."
} else {
    Write-Output "Произошла ошибка при конвертации."
}
