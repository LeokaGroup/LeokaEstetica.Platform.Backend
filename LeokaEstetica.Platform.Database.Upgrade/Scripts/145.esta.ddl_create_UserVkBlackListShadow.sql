CREATE TABLE "Access"."UserVkBlackListShadow"
(
    "ShadowId"      BIGSERIAL
        CONSTRAINT "PK_UserVkBlackListShadow_ShadowId"
            PRIMARY KEY,
    "DateCreated"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "ActionText"    VARCHAR(300)            NOT NULL,
    "ActionSysName" VARCHAR(100)            NOT NULL,
    "UserId"        BIGINT                  NOT NULL,
    "VkUserId"      BIGINT                  NOT NULL
);