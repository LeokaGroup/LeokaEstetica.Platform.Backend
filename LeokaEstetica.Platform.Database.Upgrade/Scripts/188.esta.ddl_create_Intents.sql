CREATE TABLE "Profile"."Intents"
(
    "IntentId"      SERIAL
        CONSTRAINT "PK_Intents_IntentId"
            PRIMARY KEY,
    "IntentName"    VARCHAR(200) NOT NULL,
    "IntentSysName" VARCHAR(200) NOT NULL,
    "Position"      INTEGER      NOT NULL
);