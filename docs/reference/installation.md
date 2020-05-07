# Installationsanleitung von log4TC

## Setup

Den aktuellen Release von Log4TC kann [hier](https://github.com/mbc-engineering/log4TC/releases) geladen werden. Achten sie auf die Ziel Architektur x86 bzw x64!

## Voraussetzungen

* [TwinCat 3 (min. 4022.00)](https://www.beckhoff.de/default.asp?download/tc3-download-xar.htm)

**Nur Service:**

* min Windows 7 SP1 / Windows Embedded Standard 2009
* [Microsoft .NET Framework (mindestends 4.6.1, empfohlen 4.8)](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-offline-installer)
* [ADS Router - TC1000 | TC3 ADS](https://www.beckhoff.de/default.asp?download/tc3-download-xar.htm)

## Beispiel Installation

Vorgehen zur Installation auf einem Zielsystem wie einem C6015 mit Windows 10 und einer x64 Architektur.

1. Stellen Sie sicher das alle Anwendungen geschlossen sind.
2. Kopieren des MSI [Mbc.Log4Tc.Setup(x64)v20.5.6.0](https://github.com/mbc-engineering/log4TC/releases) auf den Zielrechner. Führen Sie das MSI setup aus.
3. Akzeptieren Sie den `log4TC Software-Lizenzvertrag`.
4. Durch Klicken auf `Install` werden alle notwendigen Dateien auf das System kopiert und der log4TC Windows Service mit dem Namen `mbc log4TC Service` gestartet.

![](assets/setup_successfull.png)

# Bekannte Fehler

## Setup endet mit dem Fehler: `... Setup Wizard endet prematurely because of an error. Your system has not been modified. ...`

In diesem Fall ist ein Fehler aufgetreten.

![setup end with error](assets/setup_endwitherror.png)

Starten sie das setup erneut mit der Kommandozeile ausgeführt als Administrator. Navigieren Sie in den Ortner mit dem MSI Setup per `cd [folder]`. Geben Sie folgendes ein: `msiexec.exe /i "[setup].msi" /l*v install.log`. Wenden Sie sich anschliessend mit dem `install.log` an uns.