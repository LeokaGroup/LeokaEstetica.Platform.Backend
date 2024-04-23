CREATE TABLE "Profile"."UserIntents"
(
    "UserIntentId" BIGSERIAL
        CONSTRAINT "PK_UserIntents_IntentId"
            PRIMARY KEY,
    "IntentId"     INTEGER NOT NULL,
    "UserId"       BIGINT  NOT NULL,
    "Position"     INTEGER NOT NULL
);