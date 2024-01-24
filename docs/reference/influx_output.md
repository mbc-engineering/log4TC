# InfluxDB Ausgabe-Plugin

InfluxDB ist eine auf Zeitreihendaten spezialisierte Datenbank. Dies ermöglicht es zeitlich anfallende Daten (zyklisch und azyklisch) zu speichern und mit einer SQL-artigen Sprache abzufragen. InfluxDB ist OpenSource, eine Cloud-Lösung ist in Vorbereitung.

Zur Anzeige der Daten in InfluxDB wird eine zusätzliche Anwendung mitgeliefert, Chronograf. Hierbei handelt es sich um eine Web-Anwendung, mit dem Queries oder andere Anfragen abgesetzt werden und auch in Charts visualisiert werden können. Wir empfehlen aber für weitergehende Anwendung Grafana, das ein Influx-DB Input besitzt und zur Visualisierung mehr möglichkeiten bietet.

Log4TC wird standardmässig mit dem InfluxDB Ausgabe-Plugin ausgeliefert. Details zu InfluxDB können auf [https://www.influxdata.com/](https://www.influxdata.com/) nachgelesen werden.

## Konfiguration in log4TC

Um Meldungen an Influx ausgeben zu können, muss der log4TC-Service zuerst konfiguriert werden. Eine einfache Konfiguration (`%ProgramData%\log4TC\config\appsettings.json`) sieht wie folgt aus:

```json
{
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
      "Type": "influxdb",
      "Filter": { "Logger": "influx.*" },
      "Config": {
        "Url": "http://localhost:8086",
        "Database": "log4tc",
        "Format": "arguments"
      }
    }
  ]
}
```

Die wesentliche Konfiguration von Influx befindet sich in der `Outputs`-Liste.

```json
{
  "Type": "influxdb",
  "Filter": { "Logger": "influx.*" },   # optional zur Selektioni der Meldungen
  "Config": {
    "Url": "http://localhost:8086",     # Adresse und Port, auf dem InfluxDB Daten entgegenimmt
    "Username": "",                     # Benutzername, optional, Default kein Benutzer
    "Password": "",                     # Passwort, optional, Default kein Passwort
    "Database": "log4tc",               # Datenbank, erforderlich
    "RetentionPolicy": "",              # Retention-Policy, optional, Default Standard-Retention des Servers
    "WriteBatchSize": 1,                # Anzahl Meldungen, die zusammen geschrieben werden, optional, Default 1
    "WriteFlushIntervalMillis": 1000,   # Max. Wartezeit bevor ein nicht vollständiger Batch geschrieben wird, optional, Default 1000
    "Format": "arguments",              # Format, mit dem die Daten geschrieben werden, optiona, Default "arguments", Alternativen: "syslog"
    "SyslogFacilityCode": 16           # Nummer der verwendeten Syslog-Facility bei Format "syslog"
  }
}
```

InfluxDB kann nativ installiert werden, wir empfehlen aber den Betrieb über Docker, zumindest wenn Docker abseits der TwinCAT-Runtime installiert werden kann (Docker läuft nicht zusammen mit der TwinCAT-Runtime bzw. umgekehrt).

> [!NOTE]
> Ein Beispiel Docker-Compose befindet sich auf Github im InfluxDB-Beispielordner https://github.com/mbc-engineering/log4TC/blob/master/source/TwinCat_Examples/influx_with_message/docker-compose.yml.

## log4TC Log-Meldungen in InfluxDB

Log4TC kann Log-Meldungen in zwei Formaten in die InfluxDB schreiben.

### Format `syslog`

Bei diesen Format wird die komplette Log-Message im Syslog-Format geschrieben. In diesen Fall können die Meldungen mit dem in Chronograf integrierten LogViewer angezeigt, gefilter und gesucht werden.

> [!NOTE]
> Die Möglichkeiten zur Anzeige und Verarbeitung der Log-Messages ist relativ einfach und kann für viele Anwendungen ausreichen, vor allem wenn aus anderen Gründen Influx vorhanden ist. Für mehr Möglichkeiten zur Log-Message analyse empfehlen wir aber Graylog.

### Format `arguments`

Bei diesen Format werden nur die strukturierten Argumente einer Log-Message in die Influx-DB geschrieben. Dieses Format bietet sich an, wenn man die Daten in Charts anzeigen möchte.

Der Ablauf ist wie folgt:

* Sobald eine Meldung in der Ausgabe ankommt, wird geprüft ob sie strukturierte Argumente enthält (z.B. `Anfrage mit ErrorCode={code} abgeschlossen`). Wenn nicht, wird diese Meldung übersprungen.
* Es wird ein Measurement erzeugt mit dem Namen des Loggers.
* Der Zeitstempel entspricht dem PLC-Zeitstempel in us-Auflösung.
* Alle Context-Attribute der Log-Message werden als Tags hinzugefügt.
* Zusätzlich werden folgende Standard-Tags gesetzt:
  * **`level`** - Der Log-Level
  * **`source`** - Die Ads-Net-Id von der die Log-Message empfangen wurde.
  * **`hostname`** - Der Hostname von dem die Log-Message empfangen wurde.
  * **`taskName`** - Der Name der TwinCAT Application
  * **`taskIndex`** - Der TwinCAT Task-Index (1-x)
  * **`appName`** - Der Name der TwinCAT Application (z.B. `Port_851`)
  * **`projectName`** - Der Name des SPS-Projekts.
* Alle strukturierten Argumente werden mit ihren Namen und Wert als Felder zur Messung hinzugefügt. Im einleitenden Beispiel wäre das das Feld `code` mit dem Wert des Arguments.

Wir empfehlen alle Werte die zusammengehören in der gleichen Log-Meldung zu verschicken, da sie dann zusammen in die Messung geschrieben werden.

> [!NOTE]
> Soll eine Log-Meldung nur an Influx ausgegeben werden, kann die Meldung nur aus den Platzhaltern bestehen z.B. `{code}` anstatt `Anfrage mit ErrorCode={code} abgeschlossen`.
