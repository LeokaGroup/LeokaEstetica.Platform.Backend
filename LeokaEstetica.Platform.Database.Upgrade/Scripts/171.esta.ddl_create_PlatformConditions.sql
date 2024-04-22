CREATE TABLE dbo."PlatformConditions"
(
    "PlatformConditionId"       SERIAL
        CONSTRAINT "PK_PlatformConditions_PlatformConditionId"
            PRIMARY KEY,
    "PlatformConditionTitle"    VARCHAR(200),
    "PlatformConditionSubTitle" VARCHAR(200)      NOT NULL,
    "Position"                  INTEGER DEFAULT 0 NOT NULL
);