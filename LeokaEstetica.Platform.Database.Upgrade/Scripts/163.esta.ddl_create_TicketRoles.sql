CREATE TABLE "Communications"."TicketRoles"
(
    "RoleId"      SERIAL
        CONSTRAINT "PK_TicketRoles_RoleId"
            PRIMARY KEY,
    "RoleName"    VARCHAR(100) NOT NULL,
    "RoleSysName" VARCHAR(100) NOT NULL
);