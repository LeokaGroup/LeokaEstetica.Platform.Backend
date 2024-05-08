CREATE TABLE "Communications"."DialogMessages"
(
    "MessageId"   BIGSERIAL
        CONSTRAINT "PK_DialogMessages_MessageId"
            PRIMARY KEY,
    "Message"     TEXT                    NOT NULL,
    "Created"     TIMESTAMP DEFAULT NOW() NOT NULL,
    "DialogId"    BIGINT                  NOT NULL
        CONSTRAINT "FK_MainInfoDialogs_DialogId"
            REFERENCES "Communications"."MainInfoDialogs",
    "UserId"      BIGINT                  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "IsMyMessage" BOOLEAN                 NOT NULL
);