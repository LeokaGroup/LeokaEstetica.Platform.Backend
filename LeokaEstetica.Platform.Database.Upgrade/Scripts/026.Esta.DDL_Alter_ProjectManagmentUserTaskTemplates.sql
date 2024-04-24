CREATE TABLE dbo."Users"
(
    "UserId"                BIGSERIAL
        CONSTRAINT "PK_Users_UserId"
            PRIMARY KEY,
    "LastName"              VARCHAR(120)             DEFAULT ''::CHARACTER VARYING,
    "FirstName"             VARCHAR(120)             DEFAULT ''::CHARACTER VARYING,
    "SecondName"            VARCHAR(120)             DEFAULT ''::CHARACTER VARYING,
    "Login"                 VARCHAR(120)             DEFAULT ''::CHARACTER VARYING,
    "UserIcon"              TEXT                     DEFAULT ''::TEXT,
    "DateRegister"          TIMESTAMP WITH TIME ZONE,
    "Email"                 VARCHAR(120)                              NOT NULL,
    "EmailConfirmed"        BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "PasswordHash"          TEXT                     DEFAULT ''::TEXT NOT NULL,
    "PhoneNumber"           VARCHAR(500)             DEFAULT ''::CHARACTER VARYING,
    "PhoneNumberConfirmed"  BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "TwoFactorEnabled"      BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "LockoutEnd"            BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "LockoutEnabled"        BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "UserCode"              UUID,
    "LockoutEnabledDate"    TIMESTAMP WITH TIME ZONE,
    "LockoutEndDate"        TIMESTAMP WITH TIME ZONE,
    "ConfirmEmailCode"      UUID,
    "IsVkAuth"              BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "VkUserId"              BIGINT,
    "LastAutorization"      TIMESTAMP WITH TIME ZONE DEFAULT NOW()    NOT NULL,
    "IsMarkDeactivate"      BOOLEAN                  DEFAULT FALSE    NOT NULL,
    "DateCreatedMark"       TIMESTAMP WITH TIME ZONE DEFAULT NOW()    NOT NULL,
    "SubscriptionStartDate" TIMESTAMP WITH TIME ZONE,
    "SubscriptionEndDate"   TIMESTAMP WITH TIME ZONE,
    "IsShortLastName"       BOOLEAN                  DEFAULT FALSE    NOT NULL
);

CREATE UNIQUE INDEX "Idx_UserCode"
    ON dbo."Users" ("UserCode");

CREATE UNIQUE INDEX "Idx_Users_DateRegister"
    ON dbo."Users" ("DateRegister");

DROP TABLE IF EXISTS "Templates"."ProjectManagmentUserTaskTemplates";
CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagmentUserTaskTemplates"
(
    "UserId"     BIGINT  NOT NULL,
    "TemplateId" INT     NOT NULL,
    "IsActive"   BOOLEAN NOT NULL,
    CONSTRAINT "PK_ProjectManagmentUserTaskTemplates_TemplateId_UserId" PRIMARY KEY ("UserId", "TemplateId"),
    CONSTRAINT "FK_Users_UserId" FOREIGN KEY ("UserId") REFERENCES dbo."Users" ("UserId")
);
COMMENT ON TABLE "Templates"."ProjectManagmentUserTaskTemplates" IS 'Таблица шаблонов, которые выбрал пользователь.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."UserId" IS 'Id пользователя.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."TemplateId" IS 'Id шаблона.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."IsActive" IS 'Признак активности шаблона. Это поле нужно прежде всего для отображения всех шаблонов пользователя, что он выбирал ранее.';