CREATE TABLE "Moderation"."ProjectCommentsModeration"
(
    "ModerationId"       BIGSERIAL
        CONSTRAINT "PK_ProjectCommentsModeration_ModerationId"
            PRIMARY KEY,
    "DateModeration"     TIMESTAMP DEFAULT NOW() NOT NULL,
    "CommentId"          BIGINT                  NOT NULL
        CONSTRAINT "FK_ProjectComments_CommentId"
            REFERENCES "Communications"."ProjectComments",
    "ModerationStatusId" INTEGER                 NOT NULL
        CONSTRAINT "FK_ModerationStatuses_StatusId"
            REFERENCES "Moderation"."ModerationStatuses"
);