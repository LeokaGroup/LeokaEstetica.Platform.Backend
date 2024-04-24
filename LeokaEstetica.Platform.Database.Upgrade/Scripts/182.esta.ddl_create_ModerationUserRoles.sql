CREATE TABLE "Roles"."ModerationUserRoles"
(
    "UserRoleId" SERIAL
        CONSTRAINT "PK_ModerationUserRoles_UserRoleId"
            PRIMARY KEY,
    "RoleId"     INTEGER NOT NULL
        CONSTRAINT "FK_ModerationRoles_RoleId"
            REFERENCES "Roles"."ModerationRoles",
    "UserId"     BIGINT  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "IsActive"   BOOLEAN NOT NULL
);