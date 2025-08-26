---
date:   2020-10-21 10:54:00 +0200
categories: update log4tc v20.10.21
---
# Neuer Release log4TC: 20.10.21

## Ausgabe der Log-Meldungen in SQL-Datenbanken

Dieses Release erweitert die log4TC Ausgaben um ein weiteres Plugin: Neu können Log-Meldungen in SQL-Datenbanken geschrieben werden. Log4TC wird dazu mit drei Treibern ausgeliefert: MySql/MariaDB, Postgres, MS-SQlServer. Für die Meldungen stehen zwei verschiedene Formate zur Verfügung.

Die Dokumentation der SQL-Ausgabe befindet sich [Hier](https://mbc-engineering.github.io/log4TC/reference/sql_output.html).

Brauchen Sie Treiber für anderen Datenbanken? Wollen Sie ein anderes Schema? Zögern Sie nicht und fragen Sie uns!

Sie finden das log4TC Setup hier: [log4TC Release](https://github.com/mbc-engineering/log4TC/releases)

## Fehlerbehebungen

Der Release beseitigt folgende Fehler:

* NLog-Ausgabe wird nicht mehr initialisiert, wenn die Ausgabe nicht konfiguriert wird.
* Fehler bei der Ausgabe werden jetzt in jeden Fall in das interne log4TC-Logfile geschrieben.

## Sonstige Anpassungen

* Lizenzierung für Windows-CE möglich.

