CREATE TABLE "Access"."UserEmailBlackListShadow"
(
    "ShadowId"      BIGSERIAL
        CONSTRAINT "PK_UserEmailBlackListShadow_ShadowId"
            PRIMARY KEY,
    "DateCreated"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "ActionText"    VARCHAR(300)            NOT NULL,
    "ActionSysName" VARCHAR(100)            NOT NULL,
    "UserId"        BIGINT                  NOT NULL,
    "Email"         VARCHAR(120)            NOT NULL
);