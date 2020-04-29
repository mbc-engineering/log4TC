# Format Strings für Meldungstexte

log4TC unterstütz mehrer Arten von Platzhaltern in Meldungstexten.

## Positions Argumente

Positions Argumente werden im Meldungstext mit Nummer in geschweiften Klammern angegeben. Die Zählung startet bei 0.

Beispiel:

`'Die Verarbeitung von {0} wurde {1} abgeschlossen'`

Dieser Meldungstext hat zwei positions Argumente, die bei der Ausgabe ersetzt werden können.

## Benannte Argumente

Argumente können auch mit einem Namen benannt werden.

Beispiel:

`'Die Verarbeitung von {typ} wurde {status} abgeschlossen'`

Auch hier können im Meldungstext zwei Argumente ersetzt werden. Die Nummerierung erfolgt implizit immer von links nach rechts, also der Platzhalter `{typ}` ist das 1. Argumente, `{status}` das 2. Argument.

## Optionen für Argumente

Sowohl für positions als auch für benannte Argumente können noch weiter Optionen angegeben werden. Details dazu finden sich in [Composite Format String](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting?view=netframework-4.8), nachfolgend aber eine Übersicht mit den wichtigsten Optionen.

Der grundlegende Aufbau ist wie folgt:

> `{Index[,Ausrichtung][:Format]}
bzw.
> `{Name[,Ausrichtung][:Format]}

Die Option *Ausrichtung* ist eine Ganzzahl mit Vorzeichen, welche die bevorzugte Grösse in Zeichen für die Ausgabe angibt. Positive Werte führen zu einer rechtsbündigen Ausrichtung, negative Werte zu einer linksbündigen. Beispiel:

> `({0,5})' {0}=42 => '(   42)'`
> `({0,-5})' {0}=42 => '(42   )'`
