# SQL-Datenstrukturen

## Datenbanken

## MySql

```json
"Config": {
    "ConnectionType": "MySql.Data.MySqlClient.MySqlConnection, MySql.Data",
    "ConnectionString": "Server=dbserver;Port=3306;Database=log4tc;Uid=root;Pwd=root;"
}
```

## SimpleFlat-Scheme

### MySql

```sql
CREATE TABLE IF NOT EXISTS LogEntry (
    Id                 BIGINT AUTO_INCREMENT PRIMARY KEY,
    Source             VARCHAR(30) NOT NULL,
    Hostname           VARCHAR(30) NOT NULL,
    FormattedMessage   TINYTEXT NOT NULL,
    Logger             VARCHAR(255) NOT NULL,
    Level              ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal') NOT NULL,
    PlcTimeStamp       TIMESTAMP NOT NULL,
    ClockTimeStamp     TIMESTAMP NULL,
    TaskIndex          TINYINT NOT NULL,
    TaskName           VARCHAR(63) NOT NULL,
    TaskCycleCounter   INT NOT NULL,
    AppName            VARCHAR(63) NOT NULL,
    ProjectName        VARCHAR(63) NOT NULL,
    OnlineChangeCount  INT NOT NULL
);
```

## FullFlat-Scheme

### MySql

```sql
CREATE TABLE IF NOT EXISTS LogEntry (
    Id                 BIGINT AUTO_INCREMENT PRIMARY KEY,
    Source             VARCHAR(30) NOT NULL,
    Hostname           VARCHAR(30) NOT NULL,
    FormattedMessage   TINYTEXT NOT NULL,
    Message            TINYTEXT NOT NULL,
    Logger             VARCHAR(255) NOT NULL,
    Level              ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal') NOT NULL,
    PlcTimeStamp       TIMESTAMP NOT NULL,
    ClockTimeStamp     TIMESTAMP NULL,
    TaskIndex          TINYINT NOT NULL,
    TaskName           VARCHAR(63) NOT NULL,
    TaskCycleCounter   INT NOT NULL,
    AppName            VARCHAR(63) NOT NULL,
    ProjectName        VARCHAR(63) NOT NULL,
    OnlineChangeCount  INT NOT NULL
);

CREATE TABLE IF NOT EXISTS LogArgument (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    LogEntryId          BIGINT NOT NULL REFERENCES LogEntry(Id),
    Idx                 TINYINT NOT NULL,
    Value               TINYTEXT NOT NULL,
    UNIQUE (LogEntryId, Idx)
);

CREATE TABLE IF NOT EXISTS LogContext (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    LogEntryId          BIGINT NOT NULL REFERENCES LogEntry(Id),
    Name                VARCHAR(255) NOT NULL,
    Value               TINYTEXT NOT NULL,
    UNIQUE (LogEntryId, Name)
);
```

## FullDeep-Scheme

### MySql

```sql
CREATE TABLE IF NOT EXISTS LogSource (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    Source              VARCHAR(30) NOT NULL,
    UNIQUE (Source)
);

CREATE TABLE IF NOT EXISTS LogHostname (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    Hostname            VARCHAR(30) NOT NULL,
    UNIQUE (Hostname) 
);

CREATE TABLE IF NOT EXISTS LogOrigin (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    TaskIndex           TINYINT NOT NULL,
    TaskName            VARCHAR(63) NOT NULL,
    AppName             VARCHAR(63) NOT NULL,
    ProjectName         VARCHAR(63) NOT NULL,
    UNIQUE (TaskIndex, TaskName, AppName, ProjectName) 
);


CREATE TABLE IF NOT EXISTS LogEntry (
    Id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    SourceId            BIGINT NOT NULL REFERENCES LogSource(Id), 
    HostnameId          BIGINT NOT NULL REFERENCES LogHostname(Id), 
    FormattedMessage    TINYTEXT NOT NULL,
    Message             TINYTEXT NOT NULL,
    Logger              VARCHAR(255) NOT NULL,
    Level               ENUM('trace', 'debug', 'info', 'warn', 'error', 'fatal') NOT NULL,
    PlcTimeStamp        TIMESTAMP NOT NULL,
    ClockTimeStamp      TIMESTAMP NULL,
    Origin              BIGINT NOT NULL REFERENCES LogOrigin(Id),
    TaskCycleCounter    INT NOT NULL,
    OnlineChangeCount   INT NOT NULL
);
```
