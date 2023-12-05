CREATE TABLE IF NOT EXISTS "ProjectManagment"."UserTasks"
(
    "TaskId"        BIGINT       NOT NULL,
    "TaskStatusId"  INT          NOT NULL,
    "AuthorId"      BIGINT       NOT NULL,
    "WatcherIds"    JSONB        NULL,
    "Name"          VARCHAR(255) NOT NULL,
    "Details"       TEXT         NULL,
    "Created"       TIMESTAMP    NOT NULL DEFAULT NOW(),
    "Updated"       TIMESTAMP    NOT NULL DEFAULT NOW(),
    "ProjectId"     BIGINT       NOT NULL,
    "ProjectTaskId" BIGINT       NOT NULL,
    "ResolutionId"  INT          NOT NULL,
    "StatusId"      INT          NOT NULL,
    "TagIds"        JSONB        NULL,
    "TaskTypeId"    INT          NOT NULL,
    "ExecutorId"    BIGINT       NOT NULL,
    CONSTRAINT "PK_UserTasks_TaskId" PRIMARY KEY ("TaskId"),
    CONSTRAINT "FK_UserProjects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects"."UserProjects" ("ProjectId")
);

COMMENT ON TABLE "ProjectManagment"."UserTasks" IS 'Таблица задач пользователя.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."TaskId" IS 'Id задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."TaskStatusId" IS 'Id статуса задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."AuthorId" IS 'Id пользователя, который является автором задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."WatcherIds" IS 'Id пользователей, которые являются наблюдателями задачи.
Jsonb в виде строки (например, 1,2,3).';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."Name" IS 'Название задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."Details" IS 'Описание задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."Created" IS 'Дата создания задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."Updated" IS 'Дата обновления задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."ProjectId" IS 'Id проекта, к которому принадлежит задача.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."ProjectTaskId" IS 'Id задачи, в рамках проекта. Нужен, чтобы нумерация Id задачи шло в рамках каждого проекта.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."ResolutionId" IS 'Id резолюции.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."StatusId" IS 'Id статуса задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."TagIds" IS 'Список Id тегов задачи. В виде Jsonb.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."TaskTypeId" IS 'Id типа задачи.';
COMMENT ON COLUMN "ProjectManagment"."UserTasks"."ExecutorId" IS 'Id исполнителя задачи.';