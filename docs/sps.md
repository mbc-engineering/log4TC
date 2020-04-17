# SPS Details

## Übertragung Log-Messages

Die Log-Messages werden zu Blöcken zusammengefasst und per ADS (Client: PLC, Server: Service) übertragen.

* ADS-Port: 16150
* Index-Group: 1
* Index-Offset: 1
* ADS-Write

Ein Write kann aus einer oder mehreren Log-Messages bis zur Länge `Config.nBufferLen`bestehen. Die Log-Messages werden im Buffer direkt hintereinander gehängt.

## Aufbau einer Log-Message

Die Log-Message besteht aus einem Kopf, der immer vorhanden ist:

| Länge | Datentyp   | Zweck                                                        |
| ----- | ---------- | ------------------------------------------------------------ |
| 1     | BYTE       | Version des Formats der nachfolgenden Daten. Momentan gibt es nur die Version 1, die hier beschrieben ist. |
| 1-256 | VAR_STRING | Text der Log-Message                                         |
| 1-256 | VAR_STRING | Logger-Name                                                  |
| 2     | LOG_LEVEL  | Log-Level                                                    |
| 8     | LINT       | Filetime-Zeitstempel der SPS                                 |
| 8     | LINT       | Filetime-Zeitstempel der Uhrzeit (0=nicht vorhanden)         |
| 4     | DINT       | Taskindex des Loggers                                        |
| 1-256 | VAR_STRING | Task-Name                                                    |
| 4     | UDINT      | Task-Zykluszähler                                            |
| 1-256 | VAR_STRING | Applikationsname                                             |
| 1-256 | VAR_STRING | Projektname                                                  |
| 4     | UDINT      | Online-Change Zähler                                         |

Danach können beliebige optionale Daten hinzugefügt werden. Jedes Datum wird mit einem Byte-Kennung eingeleitet, der den Typ signalisiert.

### Ende der Log-Message

| Länge | Datentyp  | Zweck                      |
| ----- | --------- | -------------------------- |
| 1     | BYTE=0xFF | Kennung für Ende der Daten |

### Argument einer Log-Message

| Länge | Datentyp  | Zweck                       |
| ----- | --------- | --------------------------- |
| 1     | BYTE=0x01 | Kennung für Argument-Daten  |
| 1     | BYTE      | Nummer des Arguments (1-10) |
| 2     | ARG_TYPE  | Datentyp des Arguments      |
| 1-256 | *         | Daten im Format des Typs.   |

### Context einer Log-Message

| Länge | Datentyp   | Zweck                                             |
| ----- | ---------- | ------------------------------------------------- |
| 1     | BYTE=0x02  | Kennung für Context-Daten einer Log-Message       |
| 1     | BYTE       | Quelle der Daten: 0=Log-Message, 1=Logger, 2=Task |
| 1-256 | VAR_STRING | Name des Context-Attributs                        |
| 2     | ARG_TYPE   | Datentyp des Arguments                            |
| 1-256 | *          | Daten im Format des Typs.                         |



## Datentypen

| Datentyp   | Beschreibung                                                 |
| ---------- | ------------------------------------------------------------ |
| VAR_STRING | Kodiert einen Single-Byte String. Das erste Byte enthält die Anzahl Zeichen des Strings. Der String wird nicht mit einem 0-Zeichen abgeschlossen. |
| LOG_LEVEL  | Aufzählung der Loglevel: 0=Trace, 1=Debug, 2=Info, 3=Warn, 4=Error, 5=Fatal |
| ARG_TYPE   | Kennung des Argument-Typs, entspricht den Datentyp `Tc2_Utilities.E_ArgTyp`. |



