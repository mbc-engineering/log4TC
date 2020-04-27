# Benutzung von Loggern

## Was sind 'Logger'?

Log4TC orientiert sich stark an etablierte Logging-Systeme wie [NLog](https://nlog-project.org/) oder [Log4j](http://logging.apache.org/) für andere Programmiersprachen. Ein Aspekt daraus ist die Übernahme des Konzepts von *Loggern*.

Rein technisch betrachtet ist ein *Logger* ein Namen für eine oder mehrere Log-Meldungen, die durch das gesamte Logging-System transparent weitergereicht wird. *Logger* bekommen erst eine Funktion, in dem sie von Filter- und/oder Ausgabe-Plugins verwendet werden. Obwohl die Bedeutung der *Logger* grundsätzlich selbst definiert werden kann, empfiehlt es sich folgender Konvetion zu folgen.

Systeme wie NLog oder Log4j verwenden den *Logger* als Bezeichnung des vollen Klassennamen, in dem eine Log-Meldung erzeugt wird. Dieses Konvention kann analog auch für log4TC verwendet werden in dem für den *Logger* der Name des Bausteins verwendet wird, in dem die Log-Meldung erzeugt wird.

## Was sind die Vorteile von 'Logger'?

Bei allen nicht trivialen Projekten entstehen im Laufe der Zeit im Code hunderte oder tausende Log-Meldungen. Sollen diese später analysiert werden, ist es oft schwierig sich daran zu erinnern, wo eine Meldung ausgegeben wurde. Benutzt man als *Logger* den Bausteinnamen, hat man sofort eine Eingrenzung.

*Logger* können aber auch für Filter verwendet werden. Soll z.B. ein Programmteil wegen Problemen mit Logging überwacht werden, können diese Meldungen aufgrund des *Logger* in separate Log-Files ausgegeben werden.

## Wie werden Logger benutzt?

Alle Funktionen, die *Logger* unterstützten haben ein `L` im Namen:

* `F_Log` wird zu `F_LogL`
* `F_LogA1` wird zu `F_LogLA1`

Diese Funktionen haben an 2. Stelle eine zusätzlichen String-Inputparameter für den *Logger*. Wenn mehrere Log-Meldungen im gleichen Baustein ausgegeben werden, lohnt es sich eine Konstante dafür zu definieren. Der geänderte Code aus dem letzten Schritt sind mit *Logger* jetzt so aus:

```
VAR CONSTANT
	sLogger		: STRING := 'MAIN';
END_VAR
--------------------------------------------------------------
IF _TaskInfo[GETCURTASKINDEXEX()].FirstCycle THEN
	F_LogL(E_LogLevel.eInfo, sLogger, 'SPS Task gestartet.');
END_IF

fbCountTime(IN:=NOT fbCountTime.Q);
IF fbCountTime.Q THEN
	nCounter := nCounter + 1;
	F_LogLA1(E_LogLevel.eDebug, sLogger, 'Zähler geändert, neuer Wert {0}', nCounter);
END_IF

PRG_TaskLog.Call();
```

## Was passiert wenn kein 'Logger' benutzt wird?

*Logger* sind ein integrierter Bestandteil von log4TC, aus diesen Grund wird intern immer ein *Logger* benutzt, auch wenn keiner angegeben wird. Der Logger in solchen Fällen ist in `Const.sGlobalLogger` definiert und hat den Wert `'_GLOBAL_'`.

Nächster Schritt: [Integration von Context-Eigenschaften](context_usage.md)



