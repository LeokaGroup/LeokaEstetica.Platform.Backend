CREATE TABLE "Communications"."MainInfoTickets"
(
    "TicketId"       BIGSERIAL
        CONSTRAINT "PK_MainInfoTickets_TicketId"
            PRIMARY KEY,
    "TicketName"     VARCHAR(200)            NOT NULL,
    "DateCreated"    TIMESTAMP DEFAULT NOW() NOT NULL,
    "TicketStatusId" SMALLINT                NOT NULL,
    "TicketFileId"   BIGINT
);