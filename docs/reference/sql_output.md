# SQL Ausgabe-Plugin

Das SQL-Ausgabe-Plugin schreibt log4TC-Meldungen in eine relationale Datenbank. Log4TC unterstützt derzeit vier Datenbanken: MySql/MariaDB, PostgreSql und MS SQLServer.

## Vorbreitung der Datenbank

Um Meldungen an eine SQL-Datenbank ausgeben zu können muss das Schema zuerst in der Datenbank vorbereitet werden. Log4TC unterstützt momentan zwei Varianten. Die DDL's zu den jeweiligen Datenbanken befinden sich am [Ende dieses Artikels](#log4tc-sql-schema).

## Konfiguration in log4TC

Die Konfiguration für den log4TC-Service (`%ProgramData%\log4TC\config\appsettings.json`) sieht wie folgt aus:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Outputs": [
    {
      "Type": "sql",
      "Config": {
        "Driver": "MySql, Postgres oder SqlServer",
        "ConnectionString": "siehe Text",
        "Scheme": "SimpleFlat oder FullFlat"
      }
    }
  ]
}
```

**Driver**

Definiert den Treiber für den Zugriff auf die Datenbank. Momentan wird log4TC mit Treibern für folgende Datenbank ausgeliefert:

* `MySql`: [MySql](https://www.mysql.com/) oder [MariaDB](https://mariadb.com/)
* `Postgres`: [Postgres](https://www.postgresql.org/)
* `SqlServer`: [MS SQL-Server](https://www.microsoft.com/de-ch/sql-server/)

**ConnectionString**

Legt die Verbindungseinstellungen zu der ausgewählten Datenbank fest. Der String ist Abhängig von der Datenbank. Die Webseite
https://www.connectionstrings.com/ liefert eine gute Übersicht über weitergehende Parameter.

* **MySql/MariaDB**: `Server=dbserverhost;Port=3306;Database=databasename;Uid=user;Pwd=password;`
* **Postgres**: `User ID=user;Password=password;Host=dbserverhost;Port=5432;Database=databasename;Pooling=true;`
* **SqlServer**: `Server=dbserverhost,1433;Database=dbserverhost;User Id=user;Password=password;`

**Scheme**

Legt fest, in welchen Format die log4TC-Meldungen geschrieben werden sollen. Momentan werden zwei Varianten unterstützt:
* `SimpleFlat` - Einfaches Schema mit einer Tabelle; strukturierte Daten werden nicht geschrieben (nur im Message-Text)
* `FullFlat` - Schreibt die Context- und Argument-Strukturen für jede Meldung in separate Tabellen

## log4TC SQL-Schema

> [!NOTE]
> Benötigen Sie ein anderes Schema oder eine andere Datenbank? Kontaktieren Sie uns!

### `SimpleFlat`

Dies ist das einfachste Format und entspricht in etwas den Informationsgehalt von Log-Files.

Tabelle **log_entry**:

| Spalte | Bedeutung |
|--------|-----------|
| id     | Eindeutige ID (Primary-Key), wird von der Datenbank vergeben |
| source | Die Ads-Net-Id von der die Log-Message empfangen wurde. |
| hostname | Der Hostname von dem die Log-Message empfangen wurde. |
| formatted_message | Die formatierte Meldung, die in der SPS geschrieben wurde. |
| logger | Der Loggername der Log-Meldung. |
| level | Der Log-Level der Log-Meldung. |
| plc_timestamp | Der (interne) PLC-Zeitstempel der TwinCAT-Runtime, wenn die Meldung erzeugt wurde. |
| clock_timestamp | Der Zeitstempel der Windowsuhr (geringe Genaugikeit) |
| task_index | Der TwinCAT Task-Index (1-x) |
| task_name | Der Name der TwinCAT Application |
| task_cycle_counter | Der Wert des Task-Zykluszähler (alle Meldungen vom gleichen Zyklus haben den gleichen Wert) |
| app_name | Der Name der TwinCAT Application (z.B. `Port_851`) |
| project_name | Der Name des SPS-Projekts. |
| onlinechange_count | Anzahl der Online-Changes |

### FullFlat

Dieses Format enthält alle log4TC-Daten inkl. Argumente und Context in einem flachen Format. Dieses Schema besteht aus drei Tabellen:

Tabelle **log_entry**:

| Spalte | Bedeutung |
|--------|-----------|
| id     | Eindeutige ID (Primary-Key), wird von der Datenbank vergeben |
| source | Die Ads-Net-Id von der die Log-Message empfangen wurde. |
| hostname | Der Hostname von dem die Log-Message empfangen wurde. |
| formatted_message | Die formatierte Meldung, die in der SPS geschrieben wurde. |
| message | Die rohe Meldung mit Platzhaltern, so wie sie in der SPS geschrieben wurde. |
| logger | Der Loggername der Log-Meldung. |
| level | Der Log-Level der Log-Meldung. |
| plc_timestamp | Der (interne) PLC-Zeitstempel der TwinCAT-Runtime, wenn die Meldung erzeugt wurde. |
| clock_timestamp | Der Zeitstempel der Windowsuhr (geringe Genaugikeit) |
| task_index | Der TwinCAT Task-Index (1-x) |
| task_name | Der Name der TwinCAT Application |
| task_cycle_counter | Der Wert des Task-Zykluszähler (alle Meldungen vom gleichen Zyklus haben den gleichen Wert) |
| app_name | Der Name der TwinCAT Application (z.B. `Port_851`) |
| project_name | Der Name des SPS-Projekts. |
| onlinechange_count | Anzahl der Online-Changes |

Tabelle **log_argument**:

| Spalte | Bedeutung |
|--------|-----------|
| id     | Eindeutige ID (Primary-Key), wird von der Datenbank vergeben |
| log_entry_id | Referenz die Tabelle log_entry |
| idx    | Der Argument-Index in der zugehörigen Log-Message |
| value  | Der Argument-Wert als String |
| type   | Der Usprungstyp des Arguments |

Tabelle **log_context**:

| Spalte | Bedeutung |
|--------|-----------|
| id     | Eindeutige ID (Primary-Key), wird von der Datenbank vergeben |
| log_entry_id | Referenz die Tabelle log_entry |
| name    | Der Context-Name der zugehörigen Log-Message |
| value  | Der Context-Wert als String |
| type   | Der Usprungstyp des Context-Attributts |

## Anhang: DDL für Datenbank-Schemas

### MySql/MariaDB

#### SimpleFlat

```sql
CREATE TABLE IF NOT EXISTS log_entry (
    id                 BIGINT AUTO_INCREMENT PRIMARY KEY,
    source             VARCHAR(30) NOT NULL,
    hostname           VARCHAR(30) NOT NULL,
    formatted_message  TINYTEXT NOT NULL,
    logger             VARCHAR(255) NOT NULL,
    level              ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal') NOT NULL,
    plc_timestamp      TIMESTAMP NOT NULL,
    clock_timestamp    TIMESTAMP NULL,
    task_index         TINYINT NOT NULL,
    task_name          VARCHAR(63) NOT NULL,
    task_cycle_counter INT NOT NULL,
    app_name           VARCHAR(63) NOT NULL,
    project_name       VARCHAR(63) NOT NULL,
    onlinechange_count INT NOT NULL
);
```

#### FullFlat


```sql
CREATE TABLE IF NOT EXISTS log_entry (
    id                 BIGINT AUTO_INCREMENT PRIMARY KEY,
    source             VARCHAR(30) NOT NULL,
    hostname           VARCHAR(30) NOT NULL,
    formatted_message  TINYTEXT NOT NULL,
    message            TINYTEXT NOT NULL,
    logger             VARCHAR(255) NOT NULL,
    level              ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal') NOT NULL,
    plc_timestamp      TIMESTAMP NOT NULL,
    clock_timestamp    TIMESTAMP NULL,
    task_index         TINYINT NOT NULL,
    task_name          VARCHAR(63) NOT NULL,
    task_cycle_counter INT NOT NULL,
    app_name           VARCHAR(63) NOT NULL,
    project_name       VARCHAR(63) NOT NULL,
    onlinechange_count INT NOT NULL
);

CREATE TABLE IF NOT EXISTS log_argument (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    log_entry_id        BIGINT NOT NULL REFERENCES log_entry(id),
    idx                 TINYINT NOT NULL,
    value               TINYTEXT NOT NULL,
    type                ENUM('null', 'byte', 'word', 'dword', 'real', 'lreal', 'sint', 'int', 'dint', 'usint', 'uint', 'udint', 'string', 'bool', 'ularge', 'large') NOT NULL,
    UNIQUE (log_entry_id, idx)
);

CREATE TABLE IF NOT EXISTS log_context (
    id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    log_entry_id        BIGINT NOT NULL REFERENCES log_entry(id),
    name                VARCHAR(255) NOT NULL,
    value               TINYTEXT NOT NULL,
    type                ENUM('null', 'byte', 'word', 'dword', 'real', 'lreal', 'sint', 'int', 'dint', 'usint', 'uint', 'udint', 'string', 'bool', 'ularge', 'large') NOT NULL,
    UNIQUE (log_entry_id, name)
);
```

### Postgres

#### SimpleFlat

```sql
CREATE TYPE log_level_type AS ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal');

CREATE TABLE IF NOT EXISTS log_entry (
    id                 BIGSERIAL PRIMARY KEY,
    source             VARCHAR(30) NOT NULL,
    hostname           VARCHAR(30) NOT NULL,
    formatted_message  TEXT NOT NULL,
    logger             VARCHAR(255) NOT NULL,
    level              log_level_type NOT NULL,
    plc_timestamp      TIMESTAMP NOT NULL,
    clock_timestamp    TIMESTAMP NULL,
    task_index         SMALLINT NOT NULL,
    task_name          VARCHAR(63) NOT NULL,
    task_cycle_counter INT NOT NULL,
    app_name           VARCHAR(63) NOT NULL,
    project_name       VARCHAR(63) NOT NULL,
    onlinechange_count INT NOT NULL
);

```

#### FullFlat

```sql
CREATE TYPE log_level_type AS ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal');

CREATE TABLE IF NOT EXISTS log_entry (
    id                 BIGSERIAL PRIMARY KEY,
    source             VARCHAR(30) NOT NULL,
    hostname           VARCHAR(30) NOT NULL,
    formatted_message  TEXT NOT NULL,
    message            TEXT NOT NULL,
    logger             VARCHAR(255) NOT NULL,
    level              log_level_type NOT NULL,
    plc_timestamp      TIMESTAMP NOT NULL,
    clock_timestamp    TIMESTAMP NULL,
    task_index         SMALLINT NOT NULL,
    task_name          VARCHAR(63) NOT NULL,
    task_cycle_counter INT NOT NULL,
    app_name           VARCHAR(63) NOT NULL,
    project_name       VARCHAR(63) NOT NULL,
    onlinechange_count INT NOT NULL
);

CREATE TYPE log_value_type AS ENUM('null', 'byte', 'word', 'dword', 'real', 'lreal', 'sint', 'int', 'dint', 'usint', 'uint', 'udint', 'string', 'bool', 'ularge', 'large');

CREATE TABLE IF NOT EXISTS log_argument (
    id                  SERIAL PRIMARY KEY,
    log_entry_id        BIGINT NOT NULL REFERENCES log_entry(id),
    idx                 SMALLINT NOT NULL,
    value               TEXT NOT NULL,
    type                log_value_type NOT NULL,
    UNIQUE (log_entry_id, idx)
);

CREATE TABLE IF NOT EXISTS log_context (
    id                  SERIAL PRIMARY KEY,
    log_entry_id        BIGINT NOT NULL REFERENCES log_entry(id),
    name                VARCHAR(255) NOT NULL,
    value               TEXT NOT NULL,
    type                log_value_type NOT NULL,
    UNIQUE (log_entry_id, name)
);
```


### SQL-Server

#### SimpleFlat

```sql
CREATE TABLE log_entry (
    id                 INT IDENTITY PRIMARY KEY,
    source             VARCHAR(30) NOT NULL,
    hostname           VARCHAR(30) NOT NULL,
    formatted_message  TEXT NOT NULL,
    logger             VARCHAR(255) NOT NULL,
    level              CHAR(5) NOT NULL,
    plc_timestamp      DATETIME2 NOT NULL,
    clock_timestamp    DATETIME2 NULL,
    task_index         SMALLINT NOT NULL,
    task_name          VARCHAR(63) NOT NULL,
    task_cycle_counter INT NOT NULL,
    app_name           VARCHAR(63) NOT NULL,
    project_name       VARCHAR(63) NOT NULL,
    onlinechange_count INT NOT NULL
);
```

#### FullFlat

```sql
CREATE TABLE log_entry (
    id                 BIGINT IDENTITY PRIMARY KEY,
    source             VARCHAR(30) NOT NULL,
    hostname           VARCHAR(30) NOT NULL,
    formatted_message  TEXT NOT NULL,
    message            TEXT NOT NULL,
    logger             VARCHAR(255) NOT NULL,
    level              CHAR(5) NOT NULL,
    plc_timestamp      DATETIME2 NOT NULL,
    clock_timestamp    DATETIME2 NULL,
    task_index         SMALLINT NOT NULL,
    task_name          VARCHAR(63) NOT NULL,
    task_cycle_counter INT NOT NULL,
    app_name           VARCHAR(63) NOT NULL,
    project_name       VARCHAR(63) NOT NULL,
    onlinechange_count INT NOT NULL
);

CREATE TABLE log_argument (
    id                  BIGINT IDENTITY PRIMARY KEY,
    log_entry_id        BIGINT NOT NULL,
    idx                 SMALLINT NOT NULL,
    value               TEXT NOT NULL,
    type                CHAR(6) NOT NULL,
    UNIQUE (log_entry_id, idx),
    FOREIGN KEY (log_entry_id) REFERENCES log_entry(id)
);

CREATE TABLE log_context (
    id                  BIGINT IDENTITY PRIMARY KEY,
    log_entry_id        BIGINT NOT NULL,
    name                VARCHAR(255) NOT NULL,
    value               TEXT NOT NULL,
    type                CHAR(6) NOT NULL,
    UNIQUE (log_entry_id, name),
    FOREIGN KEY (log_entry_id) REFERENCES log_entry(id)
);
```

