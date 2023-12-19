# NLog Ausgabe-Plugin

Log4TC wird standardmässig mit dem NLog Ausgabe-Plugin ausgeliefert, an dem alle Log-Meldungen von log4TC weitergereicht werden. Die Detailkonfiguration kann in [NLog-Project](https://nlog-project.org/) nachgeschlagen werden.

## Konfiguration in log4TC

log4TC erwartet die Konfiguration von NLog in `%ProgramData%\log4TC\config\NLog.config`, am einfachsten kommt man in den Ordner über den Link im Startmenü, der beim Installieren von log4TC angelegt wird.

Die bei der Installation mit ausgelieferte Konfiguration sind auf die [Einführung](../gettingstarted/intro.md) ausgelegt und sollte daher für eigene Projekt angepasst werden. Nachfolgende werden zwei Konfiguration vorgestellt, die als Basis für eigene Anwendungen verwendet werden können.


### Einfaches Text-Logging

Diese Konfiguration schreibt die Log-Meldungen von log4TC in normale Text-Files, die von jeden Editor gelesen werden können (siehe auch [Tools](tools.md)).

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog
	xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	throwConfigExceptions="true"
	autoReload="true"
	internalLogLevel="Info"
	throwExceptions="true">

  <!--
	See https://github.com/nlog/nlog/wiki/Configuration-file
	for information on customizing logging rules and outputs.

  See also for targets: https://nlog-project.org/config/?tab=targets
  See also for placeholders: https://nlog-project.org/config/?tab=layout-renderers
	-->

  <extensions>
    <add assembly="Mbc.Log4Tc.Output.NLog"/>
  </extensions>

  <variable name="logdir" value="${specialfolder:folder=CommonApplicationData}\log4TC\log"/>

  <targets>
    <target name="textLogFile"
            xsi:type="File"
            createDirs="true"
            encoding="utf-8"
            archiveFileName="${logdir}/log4tc.log.{#}"
            fileName="${logdir}/log4tc.log"
            maxArchiveFiles="5"
            archiveAboveSize="10485760"
            archiveNumbering="Rolling"
            layout="${longdate}|${level:uppercase=true}|${logger}|${message}|[${mbc-all-event-properties}]">
    </target>
  </targets>

  <rules>
    <!--Levels: Trace, Debug, Info, Warn, Error, Fatal, Off-->
    <logger name="*" minlevel="Debug" writeTo="textLogFile" />
  </rules>
</nlog>
```

Die Konfiguration hat folgende Eigenschaften:
* Die Ausgabe erfolgt in `%ProgramData%\log4TC\log\log4Tc.log`. Der Pfad kann über die Variable `logdir` geändert werden.
* Das Log-File wird max. 10 MByte gross, danach wird es archiviert. Es werden max. 5 Archive aufbewahrt, bevor endgültig gelöscht wird.
* Das Ausgabeformat ist: `<PLC-Zeitstempel>|<Level>|<Logger>|<Log-Message>|[<Context-Attribute>]`.
* Es werden alle Meldungen ab Level `Debug` und höher geloggt.

### Ausgabe für Log4View

Diese Konfiguration schreibt die Log-Meldungen in ein XML-Format, dass von [Log4View](https://www.log4view.com/) gelesen werden kann.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog
	xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	throwConfigExceptions="true"
	autoReload="true"
	internalLogLevel="Info"
	throwExceptions="true">

  <!--
	See https://github.com/nlog/nlog/wiki/Configuration-file
	for information on customizing logging rules and outputs.

  See also for targets: https://nlog-project.org/config/?tab=targets
  See also for placeholders: https://nlog-project.org/config/?tab=layout-renderers
	-->

  <extensions>
    <add assembly="Mbc.Log4Tc.Output.NLog"/>
  </extensions>

  <variable name="logdir" value="${specialfolder:folder=CommonApplicationData}\log4TC\log"/>

  <targets>
    <target xsi:type="File"
            name="xmlLogFile"
            createDirs="true"
            encoding="utf-8"
            archiveFileName="${logdir}/log4tc.xml.{#}"
            fileName="${logdir}/log4tc.xml"
            maxArchiveFiles="5"
            archiveAboveSize="10485760"
            archiveNumbering="Rolling"
            layout="${mbclog4jxmlevent:includeAllProperties=true:message=${message} [${mbc-all-event-properties}]}">
    </target>
  </targets>

  <rules>
    <!--Levels: Trace, Debug, Info, Warn, Error, Fatal, Off-->
    <logger name="*" minlevel="Debug" writeTo="xmlLogFile" />
  </rules>
</nlog>
```
Die Konfiguration hat folgende Eigenschaften:
* Die Ausgabe erfolgt in `%ProgramData%\log4TC\log\log4Tc.xml`. Der Pfad kann über die Variable `logdir` geändert werden.
* Das Log-File wird max. 10 MByte gross, danach wird es archiviert. Es werden max. 5 Archive aufbewahrt, bevor endgültig gelöscht wird.
* Im Meldungstext werden noch zusätzlich alle Context-Properties eingefügt, sofern welche vorhanden sind.
* Es werden alle Meldungen ab Level `Debug` und höher geloggt.

### Ausgabe für Azure ApplicationInsight

Damit NLog die Ausgabe für [Azure ApplicationInsight](https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger) unterstütz, muss ein `ApplicationInsightsTargetLog4Tc` target wie folgt konfiguriert werden.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog
	xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	throwConfigExceptions="true"
	autoReload="true"
	internalLogLevel="Info"
	throwExceptions="true">

  <!--
	See https://github.com/nlog/nlog/wiki/Configuration-file
	for information on customizing logging rules and outputs.

  See also for targets: https://nlog-project.org/config/?tab=targets
  See also for placeholders: https://nlog-project.org/config/?tab=layout-renderers
	-->

  <extensions>
    <add assembly="Mbc.Log4Tc.Output.NLog"/>
  </extensions>

  <targets>
    <target xsi:type="ApplicationInsightsTargetLog4Tc" name="appi">
      <instrumentationKey>[YourAppiInstrementationKey]</instrumentationKey>
      <!-- Can be repeated with more Custom Properties -->
      <contextproperty name="instance" layout="plc1" />
    </target>
  </targets>

  <rules>
    <!--Levels: Trace, Debug, Info, Warn, Error, Fatal, Off-->
    <logger name="*" minlevel="Trace" writeTo="appi" />
  </rules>
</nlog>
```


## NLog-Erweiterungen

Log4TC liefert einige Erweiterungen für NLog mit.

### Layout `mbclog4jxmlevent` (Erweiterung für `log4jxmlevent`)

Dieses Layout erweitert bzw. passt das in das NLog integrierte `log4xmlevent` um folgende Eingeschaften an:

* Alle C#-spezifischen Einstellungen sind weggefallen (kein Mdc, Mdlc, Ndc, CallSite und SourceInfo).
* Die TwinCAT Task-ID wird als Thread-ID ausgegeben und kann in Log4View unter diesen Namen direkt abgelesen werden
* Das Feld `log4japp` wird mit der Quelle (z.B. `172.16.23.20.1.1:350`) aufgefüllt
* Für `AppInfo` und `Message` kann ein eigenes Layout definiert werden.

Beispiel:

```
layout="${mbclog4jxmlevent:includeAllProperties=true:message=${message} [${mbc-all-event-properties}]}">
```

Properties:
* **`IdentXml`** (Boolean) - Gibt an, ob das XML formattiert ausgegeben werden soll (Default: `false`)
* **`AppInfo`** (Layout) - Inhalt des `log4japp`-Attributs (Default: Kombination aus `_TcAppName_` und `_TcProjectName_`)
* **`Message`** (Layout) - Die auszugebene Meldung (Default: Der formatierte Meldungstext)
* **`LoggerName`** (Layout) - Der auszugebene Logger (Default: Der Loggername)
* **`IncludeAllProperties`** (Boolean) - Wenn `true` werden alle (Context-)-Eigenschaften mit ausgegeben.

### LayoutRenderer `mbc-all-event-properties` (Erweiterung für `all-event-properties`)

Dieser LayoutRender hat eine zusätzliche Option `ExcludeStandard` (Default: `true`), die verhindert, dass die Standard-Properties, die jede Meldung besitzt mit ausgegeben werden.

## log4TC Standard-Properties

Alle NLog-Meldungen bekommen unabhängig vom Context und den Argumenten folgenden Properties:

* **`_TcTaskIdx_`** - Der TwinCAT Task-Index (1-x)
* **`_TcTaskName_`** - Der Name der TwinCAT Task
* **`_TcTaskCycleCounter_`** - Der Wert des Task-Zykluszähler (alle Meldungen vom gleichen Zyklus haben den gleichen Wert)
* **`_TcAppName_`** - Der Name der TwinCAT Application
* **`_TcProjectName_`** - Der Name des TwinCAT Projekts
* **`_TcOnlineChangeCount_`** - Anzahl der Online-Changes
* **`_TcLogSource_`** - Die Quelle der Log-Meldung (AdsNetId mit AdsPort)
* **`_TcHostname_`** - Der Hostname des Rechners, vom dem die Meldung stammt

## Tipps und Rezepte

**Filtern mit Properties**

Um Log-Meldungen mit Properties zu filtern (Argument und Context), kann der NLog-Filter verwendet werden:

```xml
<logger ...>
	<filter defaultAction="Ignore">
		<when condition="${event-properties:item=foobar})" action="Log" />
	</filter>
</logger>
```

Details dazu finden sich in [Filtering log message](https://github.com/nlog/nlog/wiki/Filtering-log-messages).
