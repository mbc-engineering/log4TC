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

TODO

Nächster Schritt: [Ausgabe von Log-Meldungen mit Argumenten](argument_logging.md)



