CREATE TABLE "Access"."UserPhoneBlackListShadow"
(
    "ShadowId"      BIGSERIAL
        CONSTRAINT "PK_UserPhoneBlackListShadow_ShadowId"
            PRIMARY KEY,
    "DateCreated"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "ActionText"    VARCHAR(300)            NOT NULL,
    "ActionSysName" VARCHAR(100)            NOT NULL,
    "UserId"        BIGINT                  NOT NULL,
    "PhoneNumber"   VARCHAR(50)             NOT NULL
);