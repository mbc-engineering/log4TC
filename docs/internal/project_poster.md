# Project Poster log4tc

## Summary

### Project

*Name*: log4tc

### Team

*Project Owner*: ??

*Team Members:* mbc alle

*Stakeholders*: mbc, externe TwinCAT Entwickler

## Define the problem

### What is the problem?

Beim Programmieren von SPS-Code ist es schwierig Log-Meldungen zu erzeugen, obwohl das gerade für die SPS sehr wichtig ist, da hier häufig auf tiefer Ebene programmiert wird.

### What are the possible solutions?

Man kann über eine von TwinCAT integrierte Library Log-Meldungen absetzten, aber die ist zum einem relativen alt, schwierig im Code zu verwenden (Argumente) und in der Ausgabe sehr eingeschränkt (nur Visual Studio Output).

Man könnte auch Log-Meldungen über den TwinCAT-Eventlogger absetzten, aber diese sind realtiv schwergewichtigt, da man hier die Texte separat pflegen muss und auch die API nicht sehr einfach ist zum benutzten. Auf der Ausgabeseite gibt es auch hier nichts fertiges.

## Work through a solution

### Solutions Details

Wir stellen eine SPS-Library und eine Windows-Anwendung zur Verfügung. Die SPS-Library stellt dem Programmierer eine einfache aber trotzdem mächtige API zur Verfügung um Log-Messages (im weiteren Sinn) zu erzugen. Diese wird dann intern zur Windows-Anwendung transferiert und von dort über eine Konfiguration z.B. auf NLog ausgegeben.

### Validation

Wir haben bereits Erfahrung mit einem ähnlichen System mit dem Namen "GraylogAgent" gemacht. Dieser zeigt die prinzipielle Machbarkeit. Das System ist aber zu Eingeschränkt um darauf aufzubauen.

### Measuring Success

Wir werden die Library intern für alle mbc-SPS Projekte einsetzten, aber auch für externe SPS-Entwickler Lizenzieren.

## Ready to go

### Milestones

| Milestone Summary | Details | Dependencies | Ship Date |
| ----------------- | ------- | ------------ | --------- |
|                   |         |              |           |

### Team

| Role | Which Milestone | Availability Notes |
| ---- | --------------- | ------------------ |
|      |                 |                    |



**Ref**

* https://www.atlassian.com/team-playbook/plays/it-project-poster

