CREATE TABLE "Projects"."UserProjectsStages"
(
    "UserProjectStageId" BIGSERIAL
        CONSTRAINT "PK_UserProjectsStages_UserProjectStageId"
            PRIMARY KEY,
    "ProjectId"          BIGINT  NOT NULL
        CONSTRAINT "FK_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects",
    "StageId"            INTEGER NOT NULL
);