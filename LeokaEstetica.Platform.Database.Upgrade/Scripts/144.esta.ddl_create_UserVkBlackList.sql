CREATE TABLE "Access"."UserVkBlackList"
(
    "BlackId"  BIGSERIAL
        CONSTRAINT "PK_UserVkBlackList_BlackId"
            PRIMARY KEY,
    "UserId"   BIGINT NOT NULL,
    "VkUserId" BIGINT NOT NULL
);