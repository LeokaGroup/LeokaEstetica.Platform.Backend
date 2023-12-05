CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskHistory"
(
    "HistoryId" BIGSERIAL    NOT NULL,
    "ActionId"  INT       NOT NULL,
    "Created"   TIMESTAMP NOT NULL DEFAULT NOW(),
    "Updated"   TIMESTAMP NOT NULL DEFAULT NOW(),
    "TaskId"    BIGINT    NOT NULL,
    CONSTRAINT "PK_TaskHistory_HistoryId" PRIMARY KEY ("HistoryId"),
    CONSTRAINT "FK_UserTasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "ProjectManagment"."UserTasks" ("TaskId")
);

COMMENT ON TABLE "ProjectManagment"."TaskHistory" IS 'Таблица истории задачи. Описывает действия, которые происходили с задачей.';
COMMENT ON COLUMN "ProjectManagment"."TaskHistory"."HistoryId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskHistory"."ActionId" IS 'Id действия, которое совершили над задачей.';
COMMENT ON COLUMN "ProjectManagment"."TaskHistory"."Created" IS 'Дата создания действия.';
COMMENT ON COLUMN "ProjectManagment"."TaskHistory"."Updated" IS 'Дата обновления действия.';
COMMENT ON COLUMN "ProjectManagment"."TaskHistory"."TaskId" IS 'Id задачи, которой принадлежит комментарий.';