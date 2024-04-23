CREATE SCHEMA "Roles";

CREATE TABLE "Roles"."ModerationRoles"
(
    "RoleId"      SERIAL
        CONSTRAINT "PK_ModerationRoles_RoleId"
            PRIMARY KEY,
    "RoleName"    VARCHAR(150) NOT NULL,
    "RolSyseName" VARCHAR(150) NOT NULL
);