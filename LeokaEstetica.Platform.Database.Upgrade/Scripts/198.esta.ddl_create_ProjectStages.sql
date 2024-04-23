CREATE TABLE "Projects"."ProjectStages"
(
    "StageId"      SERIAL
        CONSTRAINT "PK_ProjectStages_StageId"
            PRIMARY KEY,
    "StageName"    VARCHAR(150) NOT NULL,
    "StageSysName" VARCHAR(150) NOT NULL,
    "Position"     INTEGER      NOT NULL
);