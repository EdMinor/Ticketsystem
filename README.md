README — Ticketsystem

Краткое описание
---------------
Ticketsystem — веб-приложение на ASP.NET Core 8 с Identity и SQLite в качестве хранилища. В проекте используются Tailwind CSS (через CLI) и сторонние фронтенд-библиотеки (Flowbite, Simple Datatables).

Ключевые сведения
------------------
- Проект: ASP.NET Core (целевой фреймворк `net8.0`).
- БД: SQLite — файл `ticketsystem.db` (настройка в `appsettings.json`, строка подключения `TicketSystemDatenbankVerbindung`).
- Identity: используется `ApplicationUser` и миграции EF Core; при старте приложение автоматически применяет миграции и выполняет сидирование (файлы `Migrations/` и `Models/DbInit.cs`).
- Файлы фронтенда: в `package.json` указаны `tailwindcss`, `@tailwindcss/cli`, `flowbite`, `simple-datatables`. Скриптов `npm run` в `package.json` нет.

Требования
----------
- .NET SDK 8.x установлен и доступен в PATH (проверьте `dotnet --version`).
- Node.js и npm (для сборки CSS/фронтенда) — опционально, если вы будете генерировать Tailwind CSS.

Быстрый запуск (локально)
------------------------
1. Откройте PowerShell в корне репозитория и выполните (устраняет отсутствие зависимостей):

```powershell
cd C:\Users\EdMin\Ticketsystem\Ticketsystem
dotnet restore
```

2. Построить проект и запустить:

```powershell
dotnet build
dotnet run
```

По умолчанию приложение выполнит миграции при старте и создаст (если нужно) `ticketsystem.db` в каталоге проекта. После старта в консоли будет URL для доступа (обычно https://localhost:5001 и http://localhost:5000).

Дополнительно: ручная работа с миграциями
---------------------------------------
Если вы хотите управлять миграциями вручную:

```powershell
cd C:\Users\EdMin\Ticketsystem\Ticketsystem
dotnet ef database update
```

(Требуется установка инструментов EF Core, если их нет: `dotnet tool install --global dotnet-ef` и пакет `Microsoft.EntityFrameworkCore.Tools` — уже указан в `.csproj`.)

Фронтенд: Tailwind CSS и статические ассеты
-----------------------------------------
Файл `package.json` содержит зависимости, но не скрипты. Если нужно генерировать CSS с Tailwind, выполните:

```powershell
cd C:\Users\EdMin\Ticketsystem\Ticketsystem
npm install
# Пример команды для генерации CSS — проверьте реальные пути к входному/выходному CSS в проекте
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
```

Если в проекте отсутствует `tailwind.config.js` или `input.css`, создайте их согласно документации Tailwind.

Учётные записи по умолчанию
-------------------------
В `Program.cs` есть комментарии с примерными быстрыми пользователями/ролями (например, `admin@demo.de` / `admin123!`). Приложение при первом запуске выполняет сидирование ролей/пользователей (функция `DbInit.SeedRolesAndUsersAsync`).

Отладка и полезные команды
--------------------------
- Проверить версию .NET:

README — Ticketsystem

Kurzbeschreibung
-----------------
Ticketsystem ist eine Webanwendung auf Basis von ASP.NET Core 8 mit Identity und SQLite als Datenbank. Für das Frontend werden Tailwind CSS (CLI) sowie die Bibliotheken Flowbite und Simple Datatables verwendet.

Wesentliche Informationen
------------------------
- Projekt: ASP.NET Core, Ziel-Framework `net8.0`.
- Datenbank: SQLite, Dateiname `ticketsystem.db` (Konfiguration in `appsettings.json`, Verbindungszeichenfolge `TicketSystemDatenbankVerbindung`).
- Identity: Klasse `ApplicationUser` und EF Core-Migrationen; das Programm wendet beim Start automatisch Migrationen an und führt das Seed-Skript aus (`Migrations/`, `Models/DbInit.cs`).
- Frontend-Abhängigkeiten: in `package.json` sind `tailwindcss`, `@tailwindcss/cli`, `flowbite` und `simple-datatables` aufgeführt; es sind jedoch keine npm-Startskripte definiert.

Anforderungen
-------------
- Installiertes .NET SDK 8.x (prüfen mit `dotnet --version`).
- Node.js und npm sind optional, nur erforderlich wenn Sie Tailwind CSS bzw. andere Frontend-Assets lokal bauen möchten.

Schneller Start (lokal)
----------------------
1. Öffnen Sie PowerShell im Ordner des Projekts und führen Sie aus:

```powershell
cd C:\Users\EdMin\Ticketsystem\Ticketsystem
dotnet restore
```

2. Projekt bauen und starten:

```powershell
dotnet build
dotnet run
```

Beim Start führt die Anwendung automatisch anstehende Migrationen aus und erstellt bei Bedarf die Datei `ticketsystem.db`. In der Konsole werden die lokalen URLs angezeigt (in der Regel https://localhost:5001 und http://localhost:5000).

Optionale: Migrationen manuell anwenden
-------------------------------------
Wenn Sie Migrationen selbst ausführen möchten:

```powershell
cd C:\Users\EdMin\Ticketsystem\Ticketsystem
dotnet ef database update
```

Hinweis: Falls `dotnet-ef` nicht installiert ist, installieren Sie das Tool global:

```powershell
dotnet tool install --global dotnet-ef
```

Frontend: Tailwind CSS und statische Assets
-----------------------------------------
`package.json` enthält die Frontend-Abhängigkeiten, jedoch keine vordefinierten Skripte. Um Tailwind CSS lokal zu generieren:

```powershell
cd C:\Users\EdMin\Ticketsystem\Ticketsystem
npm install
# Beispiel: prüfen Sie die tatsächlichen Pfade zu input.css und output.css im Projekt
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
```

Erstellen Sie bei Bedarf eine `tailwind.config.js` und eine Eingabe-CSS-Datei (`input.css`) gemäß der Tailwind-Dokumentation.

Vordefinierte Konten
--------------------
In `Program.cs` sind Beispielkonten und -rollen kommentiert (z. B. `admin@demo.de` / `admin123!`). Beim ersten Start werden Rollen und Beispielbenutzer durch `DbInit.SeedRolesAndUsersAsync` angelegt.

Nützliche Befehle und Fehlersuche
--------------------------------
- .NET-Version prüfen:

```powershell
dotnet --version
```

- Projekt bereinigen und neu bauen:

```powershell
dotnet clean
dotnet build
```

Hinweise
--------
- Sollte `dotnet run` nicht starten, prüfen Sie die Konsolenausgabe und die Fehlerlogs; häufige Ursachen sind fehlendes SDK oder Portkonflikte.
- Die Position der SQLite-Datei lässt sich in `appsettings.json` anpassen.
- Stellen Sie sicher, dass statische Dateien (CSS/JS) in `wwwroot` vorhanden und in den Razor-Views eingebunden sind.
