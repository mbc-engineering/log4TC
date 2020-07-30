# log4TC Servicekonfiguration

Der log4TC Service wird über eine JSON-Konfigurationsdatei im Pfad `%ProgramData%\log4TC\confg\appsettings.json` konfiguriert. Nach der Installation von log4TC wird eine Standardkonfiguration installiert, die alle Log-Meldungen auf NLog ausgibt.

Die Standardkonfiguration wie wie folgt aus:

```json
{
    "Logging": {
        "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
        }
    },
    "Outputs": [
        {
            "Type": "nlog",
        }
    ]
}  
```

Die Konfigurationsdatei wird auf Änderungen überwacht und automatisch neu geladen.

## Konfigurationsabschnitte

### Internes Logging

Über den Abschnitt `Logging` wird das log4TC interne Logging konfiguriert. Wir empfehlen hier die Standardkonfiguration beizubehalten. Intere Logs werden in die Datei `%ProgramData%\log4TC\internal\service.log` geschrieben. Bei Problemen kann hier kontrolliert werden, ob der Service korrekt arbeitet.

### Ausgaben

Der Abschnitt `Outputs` enthält die Konfiguration aller Ausgaben. Es können ein oder mehrere Ausgaben konfiguriert werden. Das Schema ist für eine Ausgabe ist wie folgt:

```json
"Outputs": [
    // Ausgabe 1
    {
        "Type": "Typ der Ausgabe",
        "Filter": { 
            "Logger": "Logger-Muster",
            "Level": "Log-Level"
        },
        "ExcludeFilter": { 
            "Logger": "Logger-Muster",
            "Level": "Log-Level"
        },
        "Config": {
            // Ausgabespezifisch
        }
    },
    // Ausgabe 2
    {
        // ...
    }
]
```

Mit `Type` wird die Ausgabe ausgewählt, wie z.B. `nlog`, `graylog`, `influxdb`. Ausgaben können mehrfach konfiguriert werden. Dieses Feld muss zwingend angegeben werden.

Mit `Filter` und `ExcludeFilter` können Log-Meldungen selektiert werden. Eine Log-Meldung muss den `Filter`-Kriterien entsprechen und nicht den `ExcludeFilter`-Kriterien, damit sie an die Ausgabe weitergeleitet wird. Das Kriterium `Logger` selektiert nach dem Loggernamen, es können wildcard (*) Platzhalter am Anfang und Ende verwendet werden, wie z.B. `"Logger": "influxdb.*"`. Mit `Level` wird der [Loglevel](loglevel.md) geprüft, er muss gleich oder höher sein.

Mit dem Objekt `Config` können die Ausgaben spezifisch konfiguriert werden, die Inhalte können bei der Beschreibung der Ausgaben nachgelesen werden.

Siehe dazu die Dokumentation der Ausgaben:
* [NLog](nlog_output.md)
* [Graylog](graylog_output.md)
* [InfluxDb](influx_output.md)
