CREATE TABLE IF NOT EXISTS "Moderation"."RemarksStatuses"
(
    "StatusId"      SERIAL
        CONSTRAINT "PK_StatusId"
            PRIMARY KEY,
    "StatusName"    VARCHAR(150) NOT NULL,
    "StatusSysName" VARCHAR(150) NOT NULL
);