# Integration von Context-Eigenschaften

Für log4TC sind Log-Meldungen mehr als simple Strings die die Textdateien geschrieben werden. Jede Log-Meldung besteht aus einer Anzahl von zwingend und optionalen Eigenschaften. Eine solche Eigenschaft sind die Context-Properties.

## Zweck der Context-Eigenschaften

Die Context-Eigenschaften (kurz: der Context) einer Log-Meldungen ermöglichen es direkt und indirekt zusätzliche Daten einer Log-Message mitzugeben, zu Verarbeiten, Filtern und Auszugeben. Der Context ist sehr ähnlich zu Meldung Argumenten, mit dem Unterschied, dass sie nicht immer direkt im Text erscheinen müssen.

Der Context existiert auf vier Ebenen:

* Task
* Nested Context
* (Logger) - momentan noch nicht implementiert
* Log-Message

Da der Context eine übergreifende Funktion hat, wird nachfolgende vor allem die Benutzung im SPS-Code betrachtet. Die Hauptvorteile werden in den letzten Schritten hervorgehoben.

## Benutzung

### Log-Message

TODO

