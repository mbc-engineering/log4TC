# Changelog

## [20.07.28]
### Added

* Logging für Service aktiviert (`%ProgramData%/log4Tc/internal/service-*.log`)
* Konfiguration des log4TC Dispatcher (Ausgabe-Plugins)
* Neue Ausgabe für Influx-DB (>= 1.8)
* Neue Ausgabe für Graylog

### Changed

* Setup angepasst, es ist nun eine Auswahl von Features möglich für die Szenarien PLC, PLC+Dev, Host
* Neue PLC-Library mit der Version 0.0.4

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
