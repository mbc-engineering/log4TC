# SQL-Datenstrukturen

## Simple-Scheme

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

Connection-String: `Server=myServerAddress;Port=1234;Database=myDataBase;Uid=myUsername;Pwd=myPassword;`
