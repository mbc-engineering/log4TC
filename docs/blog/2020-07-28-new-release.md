---
date:   2020-07-27 14:11:47 +0200
categories: update log4tc
---
# Neuer Release log4TC: 20.07.28

## Neue Ausgaben für Log-Meldungen und Bugfixing

Dieses Release bringt die angekündigten Ausgaben für Influx und Graylog mit. Ausserdem kann der log4TC-Service jetzt über eine Datei konfiguriert werden. 

Die neue Version steht auf [Hier](https://github.com/mbc-engineering/log4TC/releases) zum Download zur Verfügung.

### Neue SPS-Library

Die neue Library hat die Versionsnummer 0.0.5. Neben diversen Fehlerbereinigungen wurde die SPS-API um einen neuen Baustein ergänzt: `FB_LoggerLAC`. 

Mit diesen Baustein können Log-Meldungen in einem *FB* oder *PRG* abgesetzt werden, ohne das jedesmal der Loggername mitgegeben werden muss. 
Bei der Instanzierung hat man die Möglichkeit den Loggernamen automatisch ermitteln zu lassen (funktioniert nur bei *FB*s und *PRG*s) wenn man als Loggernamen die Konstante `Const.sLoggerFromInstance` angibt:

```
fbLog : FB_LoggerLAC(Const.sLoggerFromInstance);
```

Context-Eigenschaften können über die Eigenschaft `LoggerContext` einmalig hinzugefügt werden. Sie werden dann bei jeder Log-Messages dieses Instanz mitgeschrieben:

```
fbLog.LoggerContext.AddInt('orderno', 42);
```

Die eigentliche Ausgabe einer Meldung erfolgt über den direkten Aufruf:

```
fbLog(
    eLogLevel := E_LogLevel.eInfo,
    sMessage  := 'barbaz {bLog}',
    aArg1     := bLog,
);
```

Um eine bedingte Log-Meldung zu generieren, kann das Input-Argument `bExecute` benutzt werden. Nur wenn der Wert `TRUE` ist, wird eine Log-Meldung generiert.

```
fbLog(
    bExecute  := bError,
    eLogLevel := E_LogLevel.eError,
    sMessage  := 'Fehler aufgetreten',
);
```

### Ausgabe nach Influx

[InfluxDB](https://www.influxdata.com/) ist eine Open-Source Datenbank, spezialisiert auf die Verwaltung von Zeitreihendaten. Kurz gesagt, lassen sich damit verschiedene Daten (Nummerische Werte, Strings, etc.) mit einem Zeitstempel zeitlich geordnet speichern. Eine einfache Zeitreihe könnte z.B. eine Temperaturaufzeichnung sein:

* 09:00:00 -> 16°C
* 09:30:00 -> 18°C
* 11:00:00 -> 23°C

Die Abstände der Messung müssen dabei nicht regelmässig sein. Ebenso kann ein Zeitstempel auch mehrere Messwerte zugewiesen werden. Zusammengehörige Werte werden in einer Messung (*Measurement*) gespeichert.

Existieren mehrere Instanzen eines Messwerts - im Beispiel in etwas mehrere Sensoren, so können diese mit Tags gekennzeichnet werden. Ein umfangreiches Beispiel könnte damit so aussehen:

Klima:
* 09:00:00 -> Temperatur=16°C (Sensor=Aussen), Temperatur=21°C (Sensor=Innen)
* 09:30:00 -> Temperatur=18°C (Sensor=Aussen), Feuchtigkeit=30% (Sensor=Aussen)
* 11:00:00 -> Temperatur=23°C (Sensor=Innen)

Die gespeicherten Daten können mit einer SQL ähnlichen Abfragesprache selektiert werden. Um z.B. die maximale Aussentemperatur der letzten 24h abzufragen lautet die Abfrage:

```
SELECT MAX("Temperatur") FROM "Klima" WHERE "Sensor"="Aussen" AND timestamp >= now() - 24h
```

Mit der `influxdb`-Ausgabe können jetzt solche Daten direkt aus log4TC geschrieben werden. Wichtig dabei ist es, dass die zu schreibenden Daten als strukturierte Argumente in einer Log-Meldung vorliegen.

[Influx-DB Ausgabe](https://mbc-engineering.github.io/log4TC/reference/influx_output.html)

### Ausgabe nach Graylog

Für viele Anwendungen dürfte log4TC mit der NLog-Ausgabe, die lokale Dateien schreibt ausreichend sein. Sind aber mehrere Rechner mit TwinCAT im Einsatz wird das dezentrale Logging aber sehr schnell unübersichtlich und ineffizient bei der Überwachung und Fehlersuche. Für genau diesen Fall können jetzt Log-Meldungen an einen Graylog-Server zentralisiert weitergeleitet werden.

[Graylog](https://www.graylog.org/) ist ein Log-Managementsystem, das Log-Meldungen - nicht nur von log4TC - empfängt, speichert und für Analysen bereitstellt. Durch die Indexierung können auch grosse Logdaten in kürzester Zeit durchsucht werden.

Da Graylog intern eine Log-Meldung strukturiert speichert, passt dieses System nahtlos zu log4TC.

[Graylog Ausgabe](https://mbc-engineering.github.io/log4TC/reference/graylog_output.html)

