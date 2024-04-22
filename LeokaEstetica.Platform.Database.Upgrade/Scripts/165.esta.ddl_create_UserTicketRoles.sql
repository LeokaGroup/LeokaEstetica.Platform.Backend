CREATE TABLE "Communications"."UserTicketRoles"
(
    "UserRoleId" BIGSERIAL
        CONSTRAINT "PK_UserTicketRoles_UserRoleId"
            PRIMARY KEY,
    "UserId"     BIGINT  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "RoleId"     INTEGER NOT NULL
        CONSTRAINT "FK_TicketRoles_RoleId"
            REFERENCES "Communications"."TicketRoles"
);