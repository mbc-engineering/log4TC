# Beispiel für die Verwendung von OCI-Images mit Podman auf einem Beckhoff RT Linux (Debian based) mit TwinCAT Runtime

## Einleitung

Auf einer Beckhoff RT Linux Distribution (Debian based) soll das TwinCAT Runtime System native instaliert werden. In Podman sollen OCI-Images ausgeführt werden, unter anderem ein Log4TC Image. Von der TwinCAT PLC sollen Logmeldungen an den Log4TC-Service übermittelt werden.

Folgende Container sollen ausgeführt werden:
- Log4TC
- InfuxDB
- chronograf

Dieses Beispiel bezieht sich auf das bestehnde TwinCAT-Beispiel [Logging nach Influx](../influx_with_message/README.md)

ausserdem ist es angelehnt an den heise.de artikel [IoT und Edge-Computing mit Podman](https://www.heise.de/hintergrund/IoT-und-Edge-Computing-mit-Podman-9826917.html?seite=all).
## Vorraussetzungen

> Folgende Anleitung ist für eine Debian Based Distribution. Für andere Distributionen können die Befehle abweichen. Die meisten Befehle verlangen sudo-Rechte.

```bash	
apt update
apt upgrade
# https://podman.io/docs/installation
apt-get -y install podman
```

## Podman

Podman ist ein Container-Manager, der ohne Daemon-Prozess arbeitet. Er ermöglicht die Ausführung von Kubernetes-Workloads. Dies ist ideal für IIoT und Edge-Cmputeing typeschen, ressourcenschwachen kleinstrechnern. Alle zur Verfügung gestellten Funktionen sind in einer [Support-Matrix](https://docs.podman.io/en/latest/markdown/podman-kube-play.1.html#podman-kube-play-support) in der Podman-Dokumentation aufgelistet. 

## Ausführung

```bash
# Starte pod
podman kube play log4TC_pod.yaml
podman pod ps
```

Chronograf ist nun erreichbar über die IP-Adresse auf port 8888 `http://[HOST-IP]:8888`

> ADServerConsole
> - Der AdsServer ist erreichbar unter dem Port: 48898, wie in der [Doku](https://infosys.beckhoff.com/english.php?content=../content/1033/tcadscommon/12440276875.html&id=4771231825177913410) beschrieben.

## Cleanup

```bash
# entferne den pod
podman kube down log4TC_pod.yaml 
rmdir ./data
```

## FAQ

### Verbindung zum AdsServer nicht möglich

Es sollte folgende Meldung im Log4TC-Log erscheinen (Ausgabe über `podman logs -f log4tc-log4tc-service`):

```log
21:02:33 INF] Starting log4TC service.
[21:02:34 INF] Starting ADS log receiver.
[21:02:34 INF] Log receiver connected.
[21:02:34 INF] Log4Tc AdsServer with name=Log4Tc (16150); Address=172.19.13.9.1.1:16150:16150; Version=0.0 is connected!
[21:02:34 INF] Starting log dispatcher.
[21:02:34 INF] Loading output configuration.
[21:02:34 INF] Loaded output 'nlog' with filter 'Filter(*)' and exclude 'Filter(none)'.
[21:02:34 INF] Log dispatcher started.
[21:02:34 INF] Application started. Press Ctrl+C to shut down.
[21:02:34 INF] Hosting environment: Development
[21:02:34 INF] Content root path: /mnt/c/Source/github/mbc-engineering/log4TC/source/Log4Tc/Mbc.Log4Tc.Service
```
Sollte folgende Ausgabe erscheinen, ersicthlich auf Zeile 4:

```log
[20:05:15 INF] Starting log4TC service.
[20:05:15 INF] Starting ADS log receiver.
[20:05:15 INF] Log receiver connected.
[20:05:15 ERR] AmsServer connection failed with error: Connection refused [::ffff:127.0.0.1]:48898. Port 'Log4Tc' (16150)!
[20:05:15 INF] Log receiver shutdown.
[20:05:15 INF] Starting log dispatcher.
[20:05:15 INF] Loading output configuration.
[20:05:15 INF] Loaded output 'nlog' with filter 'Filter(*)' and exclude 'Filter(none)'.
[20:05:15 INF] Log dispatcher started.
[20:05:15 INF] Application started. Press Ctrl+C to shut down.
[20:05:15 INF] Hosting environment: Production
[20:05:15 INF] Content root path: /app
```

... ist kein ADS Router lokal vorhanden. Entweder einen installieren oder die AdsRouterConsoleApp verwenden!
