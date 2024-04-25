CREATE TABLE "Moderation"."ProjectsRemarks"
(
    "RemarkId"         BIGSERIAL
        CONSTRAINT "PK_RemarkId"
            PRIMARY KEY,
    "ProjectId"        BIGINT
        CONSTRAINT "FK_Projects_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects"
            ON DELETE CASCADE,
    "FieldName"        VARCHAR(100)                               NOT NULL,
    "RemarkText"       VARCHAR(500)                               NOT NULL,
    "RussianName"      VARCHAR(100)                               NOT NULL,
    "ModerationUserId" BIGINT
        CONSTRAINT "FK_Users_UserId_ModerationUserId"
            REFERENCES dbo."Users",
    "DateCreated"      TIMESTAMP    DEFAULT NOW()                 NOT NULL,
    "RemarkStatusId"   INTEGER
        CONSTRAINT "FK_Moderation_RemarksStatuses_RemarkStatusId"
            REFERENCES "Moderation"."RemarksStatuses",
    "RejectReason"     VARCHAR(300) DEFAULT ''::CHARACTER VARYING NOT NULL
);