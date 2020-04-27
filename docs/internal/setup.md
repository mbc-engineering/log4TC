# Setup Erstellung (Draft)

## How-To Erstellen der mbc_Log4TC TC3 library

1. In Der Solution Struktur unter `mbc_Log4TC Project` in den Eigenschaften->Allgemein: die Version erhöhen
2. In Der Solution Struktur unter `mbc_Log4TC Project` Rechtsklick und das Kommando 
	a. `Speichern als Bibliothek` mit dem Namen: `mbc_Log4TC_v0.0.0.library`
	b. `Speichern als Bibliothek und installieren`. Speichern der generierten Bibliothek im Repo unter: `library` mit dem Namen `mbc_Log4TC_v0.0.0.library` (Ersetze 0.0.0 mit der selben Version wie oben).
3. Commiten der neuen Bibliothek in Git für spätere Verwendung

**Manuelle Installation**
Kopieren der zuvor installierten files unter:
C:\TwinCAT\3.1\Components\Plc\Managed Libraries\mbc engineering GmbH\Log4TC\0.0.2\mbc_Log4TC_v.0.0.2.library

unter: C:\TwinCAT\3.1\Components\Plc\Managed Libraries in chache folgender Eintrag:
<Library Path="mbc engineering GmbH\Log4TC\0.0.2\mbc_Log4TC_v.0.0.2.library" Title="Log4TC" Version="0.0.2" Company="mbc engineering GmbH" DefaultNamespace="" CategoryIds="" />

## How-To Erstellen eines Setup

TBD
