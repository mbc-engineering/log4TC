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


## Cleanup

```bash
# entferne den pod
podman kube down log4TC_pod.yaml 
rmdir ./data
```
