# SQL-Datenstrukturen

## Design decisions

**Handling von UInt32**

Postgres kennt keinen native uint32 Datentyp und ist beim Aufruf gegenüber MySQL sehr genau. Aus diesen Grund
werden die uint32 Felder als int gecastet.


## Datenbanken

## MySql

```json
      "Config": {
        "Driver": "MySql",
        "ConnectionString": "Server=mbc-srv02;Port=13306;Database=log4tc_fullflat;Uid=root;Pwd=root;",
        "Scheme": "FullFlat"
      }
```

## Postgres

```json
      "Config": {
        "Driver": "Postgres",
        "ConnectionString": "User ID=postgres;Password=postgres;Host=mbc-srv02;Port=15432;Database=log4tc_fullflat;Pooling=true;",
        "Scheme": "FullFlat"
      }
```


## SimpleFlat-Scheme

### MySql

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

### Postgresql

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


## FullFlat-Scheme

### MySql

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
    LogEntryId          BIGINT NOT NULL REFERENCES log_entry(id),
    Idx                 TINYINT NOT NULL,
    Value               TINYTEXT NOT NULL,
    Type                ENUM('null', 'byte', 'word', 'dword', 'real', 'lreal', 'sint', 'int', 'dint', 'usint', 'uint', 'udint', 'string', 'bool', 'ularge', 'large') NOT NULL,
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

### Postgresql

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
