# Format Strings für Meldungstexte

log4TC unterstütz mehrer Arten von Platzhaltern in Meldungstexten.

## Positions Argumente

Positions Argumente werden im Meldungstext mit Nummer in geschweiften Klammern angegeben. Die Zählung startet bei 0. Damit Argument mit als Position ausgewertet werden, müssen **alle** Argument nummeriert sein.

Beispiel:

`'Die Verarbeitung von {0} wurde {1} abgeschlossen'`

Dieser Meldungstext hat zwei positions Argumente, die bei der Ausgabe ersetzt werden können.

## Benannte Argumente

Argumente können auch mit einem Namen benannt werden. Sobald ein Argument benannt ist, werden **alle** Argumente als benannt behandelt.

Beispiel:

`'Die Verarbeitung von {typ} wurde {status} abgeschlossen'`

Auch hier können im Meldungstext zwei Argumente ersetzt werden. Die Nummerierung erfolgt implizit immer von links nach rechts, also der Platzhalter `{typ}` ist das 1. Argumente, `{status}` das 2. Argument.

## Optionen für Argumente

Sowohl für positions als auch für benannte Argumente können noch weiter Optionen angegeben werden. Details dazu finden sich in [Composite Format String](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting?view=netframework-4.8), nachfolgend aber eine Übersicht mit den wichtigsten Optionen.

Der grundlegende Aufbau ist wie folgt:

    `{Index[,Ausrichtung][:Format]}
bzw.

    `{Name[,Ausrichtung][:Format]}

*Index* oder *Name* ist entweder die Argument-Nr (0-basiert) oder der Argument Name. Beispiel: `Von {0} bis {1}`; `Von {startTime} bis {endTime}`.

Die Option *Ausrichtung* ist eine Ganzzahl mit Vorzeichen, welche die bevorzugte Grösse in Zeichen für die Ausgabe angibt. Positive Werte führen zu einer rechtsbündigen Ausrichtung, negative Werte zu einer linksbündigen. Beispiel:

* `'({0,5})'  {0}=42 => '(   42)'`
* `'({0,-5})' {0}=42 => '(42   )'`

Die Option *Format* bestimmt wie der Typ des Arguments formattiert wird.

Beispiel für Zahlen:

* Decimal:     `({0:D4})' {0}=42   => '(0042)'`       Nur für Ganzzahltypen. Parameter: Minium Anzahl Ziffern.
* Exponential: `({0:E2})' {0}=42   => '(4.20E+001)'`  Parameter: Anzahl Nachkommastellen
* Fixed-point: `({0:F2})' {0}=42   => '(42.00)'`      Parameter: Anzahl Nachkommastellen
* Number:      `({0:N2})' {0}=4200 => '(4’200.00)'`   Mit Gruppentrenner. Parameter: Anzahl Nachkommstellen
* Percent:     `({0:P1})' {0}=0.42 => '(42.0%)'`     Parameter: Anzahl Nachkommstellen
* Hexadecimal: `({0:X4})' {0}=42   => '(002A)'`       Nur für Ganzzahltypen.
