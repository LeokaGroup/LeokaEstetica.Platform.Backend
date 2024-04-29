CREATE TABLE "Moderation"."Projects"
(
    "ModerationId"       BIGSERIAL
        CONSTRAINT "PK_Projects_ModerationId"
            PRIMARY KEY,
    "ProjectId"          BIGINT
        CONSTRAINT "FK_Projects_ProjectId"
            REFERENCES "Projects"."UserProjects"
            ON DELETE CASCADE,
    "DateModeration"     TIMESTAMP DEFAULT NOW() NOT NULL,
    "ModerationStatusId" INTEGER   DEFAULT 2     NOT NULL
        CONSTRAINT "FK_ModerationStatuses_StatusId"
            REFERENCES "Moderation"."ModerationStatuses"
);