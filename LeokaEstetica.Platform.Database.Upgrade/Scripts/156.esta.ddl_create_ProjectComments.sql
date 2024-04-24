CREATE TABLE "Communications"."ProjectComments"
(
    "CommentId"          BIGSERIAL
        CONSTRAINT "PK_ProjectComments_CommentId"
            PRIMARY KEY,
    "ProjectId"          BIGINT                  NOT NULL,
    "Comment"            TEXT                    NOT NULL,
    "IsMyComment"        BOOLEAN                 NOT NULL,
    "Created"            TIMESTAMP DEFAULT NOW() NOT NULL,
    "UserId"             BIGINT                  NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "ModerationStatusId" INTEGER                 NOT NULL
        CONSTRAINT "FK_ModerationStatuses_StatusId"
            REFERENCES "Moderation"."ModerationStatuses"
);