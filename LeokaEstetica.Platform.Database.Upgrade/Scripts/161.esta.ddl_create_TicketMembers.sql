CREATE TABLE "Communications"."TicketMembers"
(
    "MemberId" BIGSERIAL
        CONSTRAINT "PK_TicketMembers_MemberId"
            PRIMARY KEY,
    "UserId"   BIGINT                  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "Joined"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "TicketId" BIGINT                  NOT NULL
        CONSTRAINT "FK_MainInfoTickets_TicketId"
            REFERENCES "MainInfoTickets"
);