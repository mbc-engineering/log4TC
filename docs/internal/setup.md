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

**Installation mit RepTool**
Variante allgemein profile: 

```cmd
C:\TwinCAT\3.1\Components\Plc\Common>RepTool.exe --profile="c:\TwinCAT\3.1\Components\Plc\Profiles\TwinCAT PLC Control" --installLib "C:\mbc-source\mbc\log4tc\library\mbc_Log4TC_v.0.0.2.library"
```

Variante spezifisches target profile

```cmd
C:\TwinCAT\3.1\Components\Plc\Common>RepTool.exe --profile="TwinCAT PLC Control_Build_4024.0" --installLib "C:\mbc-source\mbc\log4tc\library\mbc_Log4TC_v.0.0.2.library"
```

> Wobei das Sufix `Build_4024.0` idealerweise aus der neusten Version in `C:\TwinCAT\3.1\Components\Plc\Common` besteht.

> Uninstall funktioniert nicht, bei 2. installation gleicher version wird bestehende version überschrieben

## How-To Erstellen eines Setup

TBD
