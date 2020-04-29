# Ausgabe einer einfachen Log-Meldung

## Aufruf des Loggers im MAIN

Die in der SPS erzeugten Log-Meldungen werden nicht sofort beim Aufruf eines Log-Bausteins übertragen sondern werden zunächst in einen Task-spezifischen Puffer gespeichert. Damit diese Meldungen dann an den log4Tc-Service übertragen werden, muss in **jeder** Task das log4Tc aufgerufen werden.

Der Aufruf passiert mit folgenden Code, der im Prinzip an jeder Position stehen kann, es wird aber empfohlen in am Ende des MAIN-Bausteins aufzurufen:

```
PRG_TaskLog.Call();
```

## Ausgabe einer Log-Meldung

Um eine einfache Meldung auszugeben, wird im Beispiel einen Log-Funktion aufgerufen, die einen einfachen Text ausgibt:

```
F_Log(E_LogLevel.eInfo, 'SPS Task gestartet.');
```

Der Aufruf besteht aus zwei Parametern:

* `eLogLevel`: Muss immer angegeben werden und definiert die Stufe der Log-Meldungen. Log4Tc kenn die Stufen Trace, Debug, Info, Warn, Error, Fatal. Weitergehende Informationen zu den Log-Level und ihre Bedeutung finden Sie [Hier](../reference/loglevel.md).
* `sMessage`: Gibt einen Text an der geloggt werden soll.

Damit die Log-Meldung nur einmal ausgegeben wird, wird sie in ein IF/THEN integriert. Der gesamte Code im MAIN sieht dann so aus:

```
IF _TaskInfo[GETCURTASKINDEXEX()].FirstCycle THEN
	F_Log(E_LogLevel.eInfo, 'SPS Task gestartet.');
END_IF

PRG_TaskLog.Call();
```

## Ausführen des SPS-Projekts

Um das Projekt auszuführen müssen folgende Schritte ausgeführt werden:

* Konfiguration aktivieren (TwinCAT -> Activate Configuration)
* Laden des SPS-Programms (PLC -> Login, Download)
* Starten des SPS-Programms (PLC -> Start)

## Anzeige der ausgegebenen Meldungen

Log-Meldungen werden mit der ausgelieferten Konfiguration in das Verzeichnis `%ProgramData%\log4tc\log\` abgelegt.

Tipp: Bei einer standard Windows Installation ist der Ordner `%ProgramData` (entspricht normalerweise den Pfad `C:\ProgramData`) versteckt und kann nicht im Explorer ausgewählt werden. Man kann aber den Text `%programdata%` als Pfad im Explorer eingeben und gelangt dann direkt zum Ordner.

![ProgramData](_assets/programdata.png)

Im Log-Ordner befinden sich zwei Dateien, momentan ist nur die `log4tc.log` zu beachten.

![Log-Ordner](_assets/log_folder.png)

Die Datei `log4tc.log` kann mit einem normalen Texteditor geöffnet werden (siehe auch [Tools](../reference/tools.md)):

![Erste Log-Meldung](_assets/log1.png)

Die Log-Meldung besteht aus mehreren Teilen, die durch ein `|`-Zeichen getrennt sind (Das Format einer Meldung kann über die NLog-Konfiguration fast beliebig geändert werden.).

* 1: Zeitstempel der Meldung (SPS-Zeit) mit 100ns Auflösung (abhängig von Task-Zeit)
* 2: Log-Level der Meldung, enstpricht den ersten Input-Parameter (`E_LogLevel.eInfo`)
* 3: Meldungstext

## Nächster Schritt

[Ausgabe von Log-Meldungen mit Argumenten](argument_logging.md)



