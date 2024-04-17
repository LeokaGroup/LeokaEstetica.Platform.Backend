FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Пробрасываем из контейнера порт 9992, который слушает Kestrel.
EXPOSE 9992

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Рабочая директория внутри контейнера для запуска команды ENTRYPOINT.
WORKDIR /src

# Копируем файлы для публикации нашего приложения в рабочую директорию.
COPY ["LeokaEstetica.Platform.Backend/LeokaEstetica.Platform.Backend.csproj", "LeokaEstetica.Platform.Backend/"]
COPY ["LeokaEstetica.Platform.Controllers/LeokaEstetica.Platform.Controllers.csproj", "LeokaEstetica.Platform.Controllers/"]
COPY ["LeokaEstetica.Platform.Base/LeokaEstetica.Platform.Base.csproj", "LeokaEstetica.Platform.Base/"]
COPY ["LeokaEstetica.Platform.Core/LeokaEstetica.Platform.Core.csproj", "LeokaEstetica.Platform.Core/"]
COPY ["LeokaEstetica.Platform.Models/LeokaEstetica.Platform.Models.csproj", "LeokaEstetica.Platform.Models/"]
COPY ["LeokaEstetica.Platform.Redis/LeokaEstetica.Platform.Redis.csproj", "LeokaEstetica.Platform.Redis/"]
COPY ["LeokaEstetica.Platform.Diagnostics/LeokaEstetica.Platform.Diagnostics.csproj", "LeokaEstetica.Platform.Diagnostics/"]
COPY ["LeokaEstetica.Platform.Database/LeokaEstetica.Platform.Database.csproj", "LeokaEstetica.Platform.Database/"]
COPY ["LeokaEstetica.Platform.Processing/LeokaEstetica.Platform.Processing.csproj", "LeokaEstetica.Platform.Processing/"]
COPY ["LeokaEstetica.Platform.Access/LeokaEstetica.Platform.Access.csproj", "LeokaEstetica.Platform.Access/"]
COPY ["LeokaEstetica.Platform.CallCenter/LeokaEstetica.Platform.CallCenter.csproj", "LeokaEstetica.Platform.CallCenter/"]
COPY ["LeokaEstetica.Platform.Integrations/LeokaEstetica.Platform.Integrations.csproj", "LeokaEstetica.Platform.Integrations/"]
COPY ["LeokaEstetica.Platform.Notifications/LeokaEstetica.Platform.Notifications.csproj", "LeokaEstetica.Platform.Notifications/"]
COPY ["LeokaEstetica.Platform.Messaging/LeokaEstetica.Platform.Messaging.csproj", "LeokaEstetica.Platform.Messaging/"]
COPY ["LeokaEstetica.Platform.Services/LeokaEstetica.Platform.Services.csproj", "LeokaEstetica.Platform.Services/"]
COPY ["LeokaEstetica.Platform.Finder/LeokaEstetica.Platform.Finder.csproj", "LeokaEstetica.Platform.Finder/"]
COPY ["LeokaEstetica.Platform.ProjectManagment.Documents/LeokaEstetica.Platform.ProjectManagment.Documents.csproj", "LeokaEstetica.Platform.ProjectManagment.Documents/"]

# Восстановим зависимости.
RUN dotnet restore "LeokaEstetica.Platform.Backend/LeokaEstetica.Platform.Backend.csproj"
COPY . .
WORKDIR "/src/LeokaEstetica.Platform.Backend"
RUN dotnet build "LeokaEstetica.Platform.Backend.csproj" -c Release -o /app/build

# Строим образ и создаем контейнер.
FROM build AS publish
RUN dotnet publish "LeokaEstetica.Platform.Backend.csproj" -c Release -o /app/publish

# При старте контейнера поднимаем основной модуль приложения.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeokaEstetica.Platform.Backend.dll"]
