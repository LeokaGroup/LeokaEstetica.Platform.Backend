CREATE TABLE IF NOT EXISTS "Moderation"."Users"
(
    "Id"           BIGSERIAL
        CONSTRAINT "PK_Users_Id"
            PRIMARY KEY,
    "UserId"       BIGINT
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "DateCreated"  TIMESTAMP DEFAULT NOW() NOT NULL,
    "UserRoleId"   INTEGER
        CONSTRAINT "FK_Roles_UserRole"
            REFERENCES "Roles"."ModerationRoles",
    "PasswordHash" TEXT                    NOT NULL
);