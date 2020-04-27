# Log-Level

Das log4Tc kennt sechs Log-Level:

* Trace
* Debug
* Info
* Warn
* Error
* Fatal

Die Log-Level sind geordnet, Trace hat die kleinste Ordnung, Fatal die grösste.

Grundsätzlich können die Bedeutung der Log-Level frei definiert werden, es ist aber empfehlenswert, wenn diese Projekt- oder Unternehmensweit definiert wird.

Log4Tc empfiehlt diese Richtlinien für die Benutzung von Log-Level:

## Level: `Fatal`

Der `Fatal`-Level sollte für Fehler verwendet werden, die verhindern, das ein Programm komplett oder zum grossen Teil korrekt ausgeführt werden kann. Log-Meldungen von dieser Stufe bedeuten i.d.R. sofortiges Handeln und werden meist auch direkt weitergeleitet.

Beispiel: Der Safety-Controller konnte wegen eines HW-Fehlers nicht initialisiert werden.

## Level: `Error`

Der `Error`-Level kennzeichnet Meldungen, die von Fehlern stammen, die i.d.R. auch zu Problemen im Programm führen. Im Gegensatz zum `Fatal`-Level sind hier aber nur Teile der Software betroffen.

Auch diese Meldungen werden normalerweise direkt an einen Operator/Service weitergeleitet.

Beispiele: Kommunikation zu einem übergeordneten System ausgefallen.

## Level: `Warn`

Zustände, die noch nicht zu einem Fehler führen, aber bereits ein baldiges Eingreifen eines Users erfordern, können mit `Warn` gemeldet werden. Normallerweise läuft die Software noch problemlos weiter. Die Reaktion auf diese Meldungen ist häufig verzögert.

Beispiel: Zu wenig Kühlmittel im System.

## Level:`Info`

Der `Info`-Level soll wichtige Zustandsänderung der Software protokollieren. Häufig sind diese Informationen wichtig um Fehler besser zuordnen zu können.

Beispiel: Ein neues Rezept mit der ID 42 wurde geladen.

## Level: `Debug`

Dieser Level wird häufig von Entwicklern verwendet um weitere detaillierte Zustandsänderungen zu verfolgen. Im Normalfall sind diese Information nur für Entwickler notwendig.

Beispiel: Der Aufruf des Sendebaustein hat 3.2s benötigt.

## Level: `Trace`

Der `Trace`-Level wird ebenfalls von Entwickler verwendet um weitergehende interne Zustände zu verfolgen. Entwickler verwenden diesen Level für die Analyse von bestehenden Problemen. Log-Messages dieses Levels werden häufig nicht dauerhaft abgespeichert.



