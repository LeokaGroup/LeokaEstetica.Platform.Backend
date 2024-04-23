CREATE SCHEMA "Logs";

CREATE TABLE "Logs"."Logs"
(
    "LogId"       BIGSERIAL
        PRIMARY KEY,
    "Application" VARCHAR(200),
    "Timestamp"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "Level"       VARCHAR(5)              NOT NULL,
    "Message"     TEXT                    NOT NULL,
    "Logger"      VARCHAR(300),
    "CallSite"    TEXT,
    "Exception"   TEXT
);