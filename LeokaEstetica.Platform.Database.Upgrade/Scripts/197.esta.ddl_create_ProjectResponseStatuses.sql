CREATE TABLE "Projects"."ProjectResponseStatuses"
(
    "StatusId"      SERIAL
        CONSTRAINT "PK_ProjectResponseStatuses_StatusId"
            PRIMARY KEY,
    "StatusName"    VARCHAR(150) NOT NULL,
    "StatusSysName" VARCHAR(150) NOT NULL
);