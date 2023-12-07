CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskComments"
(
    "CommentId" BIGSERIAL    NOT NULL,
    "Comment"   TEXT      NOT NULL,
    "Created"   TIMESTAMP NOT NULL DEFAULT NOW(),
    "Updated"   TIMESTAMP NOT NULL DEFAULT NOW(),
    "TaskId"    BIGINT    NOT NULL,
    "AuthorId"  BIGINT    NOT NULL,
    CONSTRAINT "PK_TaskComments_CommentId" PRIMARY KEY ("CommentId"),
    CONSTRAINT "FK_UserTasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "ProjectManagment"."UserTasks" ("TaskId")
);

COMMENT ON TABLE "ProjectManagment"."TaskComments" IS 'Таблица комментариев к задаче.';
COMMENT ON COLUMN "ProjectManagment"."TaskComments"."CommentId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskComments"."Comment" IS 'Комментарий.';
COMMENT ON COLUMN "ProjectManagment"."TaskComments"."Created" IS 'Дата создания комментария.';
COMMENT ON COLUMN "ProjectManagment"."TaskComments"."Updated" IS 'Дата обновления комментария.';
COMMENT ON COLUMN "ProjectManagment"."TaskComments"."TaskId" IS 'Id задачи, которой принадлежит комментарий.';
COMMENT ON COLUMN "ProjectManagment"."TaskComments"."AuthorId" IS 'Id пользователя, который создал комментарий.';