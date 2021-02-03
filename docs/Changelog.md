# Changelog

## [21.02.03]
### Added
- Neue ANY-Datentypen fürs Logging: TIME, LTIME, DATE, DATE_AND_TIME, TIME_OF_DAY, ENUM (numerisch), WSTRING

### Fixed
- Einige Startup Probleme behoben

### SPS-Library 0.2.0
- Lizenz Prüfung entfernt

### SPS-Library 0.1.0

- Neue Funktion zum einfachen loggen mit ContextBuilder jedoch ohne Angabe des Loggers: `F_LogC`
- Erlauben E_LogLevel Wert als String zu konvertieren mit `{attribute 'to_string'}`
- 0-Copy Logging: Log-Meldungen werden direkt in den Sendebuffer geschrieben
- Neue ANY-Datentypen fürs Logging: TIME, LTIME, DATE, DATE_AND_TIME, TIME_OF_DAY, ENUM (numerisch), WSTRING

## [20.10.21]
### SPS-Library 0.0.6
* Lizenzierung für Windows-CE Geräte (keine OEM möglich)
### SPS-Library 0.0.8
* Lizenzierung Bugfix für Lizenzierungsklemme

## [20.10.15]
### Added
* Neuer Output für SQL-Datenbanken

### Changed
* `LogEntry` können jetzt als Collection im Output statt einzeln verarbeitet werden
* Influx-DB Output wurde angepasst, dass mehr als eine LogEntry gesendet wird

### Fixed
* Fehler im async-Handling beim Verarbeiten der Meldungen behoben; der Bug führte dazu, das im Fehlerfall Exceptions nicht geloggt wurden
* NLog wurde initialisiert, auch wenn das Output-Plugin nicht konfiguriert wurde


## [20.07.28]
### Added

* Logging für Service aktiviert (`%ProgramData%/log4Tc/internal/service-*.log`)
* Konfiguration des log4TC Dispatcher (Ausgabe-Plugins)
* Neue Ausgabe für Influx-DB (>= 1.8)
* Neue Ausgabe für Graylog

### Changed

* Setup angepasst, es ist nun eine Auswahl von Features möglich für die Szenarien PLC, PLC+Dev, Host
* Neue PLC-Library mit der Version 0.0.5

### Fixed

* Bugfix PLC: Context.AddString verwendet jetzt dir korrekt Stringlänge
* Bugfix PLC: Log-Argumente wurden nicht korrekt übergeben (ab Argumente 3)
* Im Context überschreiben jetzt gleiche Namen den vorherigen Wert

## [20.05.06]
### Added
- Log4Tc Initial Version

---

> [!NOTE]
> All wichtigen Änderungen zu diesem Projekt werden in dieser Datei dokumentiert.
>
> Das Release Versionierungsformat folgt dem Schema **Jahr.Monat.Tag.Patch**. Zum Beispiel: 20.05.06, 19.07.30.0, 19.07.30.1.
> 
> Es gibt folgende änderungstypen:
> - **Added** for new features.
> - **Changed** for changes in existing functionality.
> - **Deprecated** for soon-to-be removed features.
> - **Removed** for now removed features.
> - **Fixed** for any bug fixes.
> - **Security** in case of vulnerabilities.
