# Logging nach Graylog

## Vorraussetzungen

### Graylog

Graylog wird über Docker gestartet, daher muss Docker (z.B. [Docker Desktop](https://www.docker.com/products/docker-desktop)) installiert sein.

Gestartet werden die Container wie folgt:
* Kommandozeile öffnen
* In das Verzeichnis wechseln, in dem sich `docker-compose.yml` befindet
* `docker-compose up -d` Ausführen

### Konfiguration log4TC Service

Auf einem Rechner (z.B. auf dem Influx und Chronograf läuft) muss log4TC installiert sein. Für dieses Beispiel kann die Konfiguration `appsettings.json` in den Ordner `%ProgramData%\log4TC\config` kopiert werden.


### SPS-Projekt

Im Ordner `Plc` befindet sich ein Beispiel SPS-Projekt das einige simulierte Log-Meldungen absetzte. Falls die SPS auf einem anderen Rechner als der log4TC-Service läuft, muss zwischen den beiden Rechnern eine Route eingerichtet werden. Im `MAIN`-Baustein muss in der Ziele `PRG_TaskLog.Init('192.168.56.1.1.1');` die AMS-Net-ID des log4TC-Service Rechners eingetragen werden.

## 



INFO : org.graylog2.bootstrap.ServerBootstrap - Graylog server up and running.

* System -> Inputs
* "GELF UDP" -> Launch new input
* Global