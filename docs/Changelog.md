 <div id="google_translate_element"></div>

# Changelog
## [25.08.26]
### Added
- Stefans blogs hinzugefügt inklusive [Getting Started Video](https://www.youtube.com/embed/aXccFd3cIY0)

### Fixed
- Service Startup error `(Mbc.Log4Tc.Receiver.AdsLogReceiver) Log receiver shutdown.` wurde behoben.
- fixed Nlog.config template 
  
### Changed
- Setup OS Requirement auf Windows 10 erhöht wegen .NET 8.0 Anforderung,
- Setup TwinCAT Lib installation nur für 2024.
- Neuste Beckhoff ADS Library 6.2.485 wird verwendet
- Neuste Microsoft.ApplicationInsights.WorkerService Library 2.23.0 wird verwendet
- SPS Library umbenannt für um klar die TC Version zu kennzeichnen.

## [25.02.07]
### Fixed
- Fixed System.MethodAccessException in Log4JXmlEventLayoutRenderer bei Verwendung des NLog Target Layout mbclog4jxmlevent: `layout="${mbclog4jxmlevent:includeAllProperties=true:message=${message} [${mbc-all-event-properties}]}">`

### Changed
- Neuste Beckhoff ADS Library 6.2.244 wird verwendet
- Dependency Updates

### SPS-Library 0.3.0
- Bibliothek wurde mit TwinCAT v3.1.4026 erstellt. Lasst uns sehen ob die Kompatibilität gegeben ist. Der Funktionsumfang ist noch gleich wie in der Version 0.2.1.

## [24.12.08]
### Added
- Dokumentation steht nun auch als PDF zur Verfügung

### Changed
- Es wird nun .NET 8.0 verwendet und es ist keine Abhängigkeiten mehr zum .NET Full Framework notwendig. Die unterstützten Windows Versionen sind ab Windows 10 1607. Siehe Microsoft [Dokumentation](https://github.com/dotnet/core/blob/main/release-notes/8.0/supported-os.md#windows)
- Neuste Beckhoff ADS Library 6.1.304 wird verwendet
- Für das Windows MSI wird neu WIX 5 verwendet

### Fixed 
- #15 - Links in Dokumentation korrigiert

### Security
- vulnerabilities behoben

### Added BETA
- Es wird nun Linux x64 und ARM-x64 unterstützt. Der Configuration Pfad ist `/etc/log4tc/config` anstelle `%programdata%/log4TC/config` und für alle Logdateien `/var/log/log4tc` anstelle `%programdata%/log4TC/log` und `%programdata%/log4TC/internal`. Ausserdem wird auf Linux die [AdsRouterConsoleApp](https://github.com/Beckhoff/TF6000_ADS_DOTNET_V5_Samples/tree/main/Sources/RouterSamples/AdsRouterConsoleApp) notwendig um auf Log4TC zuzugreifen. Es ist eine Source code Kopie von Beckhoff unter 'source/AdsRouterConsoleApp' verfügbar. Die theoretisch unterstütze Linux Versionen sind in der [Microsoft Dokumentation](https://github.com/dotnet/core/blob/main/release-notes/8.0/supported-os.md#linux) aufgeführt.
- Es wird ein experimentelles Docker Image für Log4TC bereitgestellt mit dem Namen `ghcr.io/mbc-engineering/log4tc:latest`.
- Siehe Beispiel `influx_on_beckhoff-rt-linux`

### Known Issues
- Das MSI Setup unterstütz TwinCAT 4026 nicht.

## [24.01.18]
### Added
- NLog unterstützt nun auch die Ausgabe für Azure ApplicationInsight über das neue nlog Target `ApplicationInsightsTargetLog4Tc`.

### Security
- Update SQLClient

## [21.04.17]
### Added
- Neue ANY-Datentypen fürs Logging: TIME, LTIME, DATE, DATE_AND_TIME, TIME_OF_DAY, ENUM (numerisch), WSTRING

### Fixed
- Einige Startup Probleme behoben
- Setup of the library works now with TwinCat 4024 shell 

### Changed
- Einige Logeinträge

### SPS-Library 0.2.1
- Lizenz Prüfung entfernt
- Library ist nun mbc_Log4TC.library anstelle von mbc_Log4TC.compiled-library
- E_Scope nun public verwendbar

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
