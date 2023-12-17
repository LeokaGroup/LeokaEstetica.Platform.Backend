CREATE TABLE IF NOT EXISTS "ProjectManagment"."Header"
(
    "HeaderId"   SERIAL,
    "ItemName"   VARCHAR(200) NOT NULL,
    "ItemUrl"    TEXT         NULL,
    "Position"   INT          NOT NULL DEFAULT 0,
    "HeaderType" VARCHAR(50)  NOT NULL,
    "Items"      JSONB        NOT NULL,
    "HasItems"   BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsDisabled" BOOLEAN      NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_ProjectManagment_Header_HeaderId" PRIMARY KEY ("HeaderId")
);