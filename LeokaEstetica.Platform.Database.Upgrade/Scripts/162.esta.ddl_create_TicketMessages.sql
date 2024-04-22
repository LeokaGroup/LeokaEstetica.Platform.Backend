CREATE TABLE "Communications"."TicketMessages"
(
    "MessageId"   BIGSERIAL
        CONSTRAINT "PK_TicketMessages_MessageId"
            PRIMARY KEY,
    "TicketId"    BIGINT                  NOT NULL
        CONSTRAINT "FK_TicketMessages_TicketId"
            REFERENCES "MainInfoTickets",
    "Message"     TEXT                    NOT NULL,
    "DateCreated" TIMESTAMP DEFAULT NOW() NOT NULL,
    "UserId"      BIGINT                  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users"
);