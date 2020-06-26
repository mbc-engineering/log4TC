# Erste Schritte

Diese geführte Tour stellt die grundlegenden Konzepte und Features von log4TC in einem kleinen zusammenhängenden Projekt vor.

## Vorraussetzungen

Damit log4Tc richtig benutzt werden kann, sollte der Aufbau bekannt sein. Es wird dabei unterschieden zwischen den Komponenten `log4TC TwinCat 3 Bibliothek` und dem `log4TC Service`.

![architektur](assets/architektur.png)

Folgende Voraussetzungen haben die beiden Komponenten:

**log4TC TwinCat 3 Bibliothek**

* [TwinCat 3 (min. 4022.00)](https://www.beckhoff.de/default.asp?download/tc3-download-xae.htm)

**log4TC Service**

* min Windows 7 SP1 / Windows Embedded Standard 2009
* [Microsoft .NET Framework (Mindestends 4.6.1, empfohlen 4.8)](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-offline-installer)
* [ADS Router - TC1000 | TC3 ADS](https://www.beckhoff.de/default.asp?download/tc3-download-xar.htm)

Beim [Installieren](../reference/installation.md) von log4TC wird eine Default-Konfigurationsdatei mit installiert. Für die nachfolgenden Beispiele wird davon ausgegangen, dass diese Konfiguration aktiv ist.

Für diese Einführung wird ausserdem vorrausgesetzt, dass die SPS und der log4TC auf dem *gleichen* Rechner laufen (Testlizenz ist ausreichend). Dies ist keine Einschränkung von log4TC sondern eine Vereinfachung.

## Übersicht

Die Einführung geht schrittweise vor. Es wird empfohlen beim ersten Kontakt mit log4TC alle Schritte nacheinander selbst auszuprobieren.

1. [TwinCAT Projekt anlegen](create_twincat_project.md)
2. [log4TC-Library hinzufügen](add_log4tc_lib.md)
3. [Ausgabe einer einfachen Log-Meldung](simple_logging.md)
4. [Ausgabe von Log-Meldungen mit Argumenten](argument_logging.md)
5. [Benutzung von Loggern](logger_usage.md)
6. [Integration von Context-Eigenschaften](context_usage.md)
7. [Log-Meldungen mit Log4View beobachten](tools_log4view.md)
8. [Protokollierung von strukturierten Werten](write_structured_values.md)
9. [Logging nach Influx](logging_to_influx.md)

---

## Nächster Schritt

[TwinCAT Projekt anlegen](create_twincat_project.md)
