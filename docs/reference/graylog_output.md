# Graylog Ausgabe-Plugin

Graylog ist ein Log-Managementsystem, das zentrall Log-Meldungen entgegennimmt, speichert und zur Echtzeitanalyse bereitstellt. Graylog benutzt intern Elasticsearch und lässt sich horizontal skalieren, aber auch nur auf einem einzelnen Rechner installieren. Graylog kann als Open Source oder als Enterprise Version eingesetzt werden.

Log4TC wird standardmässig mit dem Graylog Ausgabe-Plugin ausgeliefert. Details zur Graylog können auf https://docs.graylog.org/ nachgelesen werden.

## Konfiguration in log4TC

Um Meldungen an Graylog ausgeben zu können, muss der log4TC-Service zuerst konfiguriert werden. Eine einfache Konfiguration (`%ProgramData%\log4TC\config\appsettings.json`) sieht wie folgt aus:

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
  {
    "Outputs": [
      {
        "Type": "graylog",
        "Config": {
          "GraylogHostname": "localhost"
        }
      }
    ]
  }
}
```

Die wesentliche Konfiguration der Graylogausgabe befindet sich in der `Outputs`-Liste.

```json
{
    "Type": "graylog",
    "Config": {
        "GraylogHostname": "localhost", # Hostname oder IP, optional, Default "localhost"
        "GraylogPort": 12201,           # UDP-Port auf Graylog-Server, optional, Default 12201
        "GelfCompression": "Gzip",      # GELF-Kompression, optional, Default gzip. Alternativen: "None"
    }
}
```

Log4TC kommuniziert mit dem Graylog-Server über Gelf-UDP, daher muss auf dem Server ein enstprechender Input konfiguriert werden, was häufig schon der Fall ist.

Graylog kann nativ installiert werden, wir empfehlen aber den Betrieb über Docker, zumindest wenn Graylog abseits der TwinCAT-Runtime installiert werden kann (Docker läuft nicht zusammen mit der TwinCAT-Runtime bzw. umgekehrt). 

> [!NOTE]
> Ein Beispiel Docker-Compose befindet sich auf Github im Graylog-Beispielordner https://github.com/mbc-engineering/log4TC/blob/master/source/TwinCat_Examples/graylog/docker-compose.yml.

## log4TC Log-Meldungen in Graylog

Log4TC versucht bei der Ausgabe in Graylog alle strukturierten Elemente zu erhalten. Eine Graylog-Meldung hat folgende Felder:

* **`appName`** - Der Name der TwinCAT Application (z.B. `Port_851`)
* **`clockTimestamp`** - Der Zeitstempel der Windowsuhr (geringe Genaugikeit)
* **`fullMessage`** - Die Log-Meldung ohne Variablenersetzung. Kann für Suche und Selektion verwendet werden um gleichartige Meldungen zu selektieren.
* **`level`** - Der Log-Level als Syslog-Wert. Graylog kann diesen Wert mit Decorators in Text umwandeln
* **`logger`** - Der Loggername der Log-Meldung.
* **`message`** - Die Log-Meldung mit Variablenersetzungen.
* **`onlineChangeCount`** - Anzahl der Online-Changes
* **`projectName`** - Der Name des SPS-Projekts.
* **`source`** - Die Ads-Net-Id von der die Log-Message empfangen wurde.
* **`hostname`** - Der Hostname von dem die Log-Message empfangen wurde.
* **`taskCycleCounter`** - Der Wert des Task-Zykluszähler (alle Meldungen vom gleichen Zyklus haben den gleichen Wert)
* **`taskIndex`** - Der TwinCAT Task-Index (1-x)
* **`taskName`** - Der Name der TwinCAT Application
* **`timestamp`** - Der PLC-Zeitstempel (hone Genauigkeit)
