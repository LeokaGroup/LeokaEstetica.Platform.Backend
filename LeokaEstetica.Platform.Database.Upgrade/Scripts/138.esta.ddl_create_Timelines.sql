CREATE TABLE dbo."Timelines"
(
    "TimelineId"       SERIAL
        CONSTRAINT "PK_TimelineId"
            PRIMARY KEY,
    "TimelineTitle"    VARCHAR(200)       NOT NULL,
    "TimelineText"     TEXT               NOT NULL,
    "TimelineSysType"  VARCHAR(150)       NOT NULL,
    "TimelineTypeName" VARCHAR(150),
    "Position"         SMALLINT DEFAULT 0 NOT NULL
);