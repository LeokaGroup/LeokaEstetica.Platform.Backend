CREATE TABLE "Communications"."TicketStatuses"
(
    "StatusId"      SERIAL
        CONSTRAINT "PK_TicketStatuses_StatusId"
            PRIMARY KEY,
    "StatusName"    VARCHAR(50) NOT NULL,
    "StatusSysName" VARCHAR(50) NOT NULL
);