CREATE TABLE "Access"."UserPhoneBlackList"
(
    "BlackId"     BIGSERIAL
        CONSTRAINT "PK_UserPhoneBlackList_BlackId"
            PRIMARY KEY,
    "UserId"      BIGINT      NOT NULL,
    "PhoneNumber" VARCHAR(50) NOT NULL
);