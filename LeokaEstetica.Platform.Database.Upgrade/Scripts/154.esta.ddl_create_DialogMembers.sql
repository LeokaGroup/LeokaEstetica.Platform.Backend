CREATE TABLE "Communications"."DialogMembers"
(
    "MemberId" BIGSERIAL
        CONSTRAINT "PK_MainInfoDialogs_MemberId"
            PRIMARY KEY,
    "Joined"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "DialogId" BIGINT                  NOT NULL
        CONSTRAINT "FK_MainInfoDialogs_DialogId"
            REFERENCES "Communications"."MainInfoDialogs",
    "UserId"   BIGINT                  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users"
);