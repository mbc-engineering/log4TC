# Design Details

## Log-Event

Der *Log-Event* beschreibt die Datenstruktur die beim Logging erfasst wird und über die interne Schnittstelle von der Echtzeit in die nicht Echtzeit zur weiteren Überarbeitung übertragen wird.

Ein Log-Event besteht aus folgenden Daten:

|Feld|Zweck|
|---|---|
|Log-Message|Enthält die eigentliche Log-Message als primitiven Datentyp (String, aber auch z.B. Integer). Dieses Feld ist zwingend.|
|Log-Name|Name des Loggers, der die Log-Message auslöst. Wie bei Log-Systemen üblich kann der Name hierarchisch mit "." gegliedert werden|
|Log-Level|Die wichtigkeit des Log-Events. Dient hauptsächlich zur Filterung. Der Level besteht aus einer vordefinierten Menge (Enum).|
|TC-Timestamp|Der TwinCAT-Zeitstempel in der vollen Auflösung.|
|PC-Timestamp|Der PC-Zeitstempel der normalen Uhr.|
|Message Arguments|Enthält die Argumente der Message, sofern welche vorhanden sind.|
|Message Attributes|Enthält optionale Attribute um eine Log-Message genauer zu beschreiben bzw. eine Context zu definieren.|







TODO:
* Kann der TwinCAT-Zeitstempel durch Ticks noch genauer definiert werden?