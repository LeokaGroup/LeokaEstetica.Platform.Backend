CREATE TABLE "Access"."UserEmailBlackList"
(
    "BlackId" BIGSERIAL
        CONSTRAINT "PK_UserEmailBlackList_BlackId"
            PRIMARY KEY,
    "UserId"  BIGINT       NOT NULL,
    "Email"   VARCHAR(120) NOT NULL
);