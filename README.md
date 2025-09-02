README — Ticketsystem

Kurzbeschreibung
-----------------
Ticketsystem ist eine Webanwendung auf Basis von ASP.NET Core 8 mit Identity und SQLite als Datenbank. Für das Frontend werden Tailwind CSS (CLI) sowie die Bibliotheken Flowbite und Simple Datatables verwendet.

Die Anwendung wurde im Rahmen einer Projektarbeit zum Abschluss des Moduls "C#" entwickelt. An der Entwicklung waren beteiligt: Mo, Suat, Ahmed, Eduard.

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
