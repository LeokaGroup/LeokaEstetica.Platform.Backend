CREATE TABLE IF NOT EXISTS "ProjectManagment"."ProjectTasks"
(
    "TaskId"        BIGSERIAL       NOT NULL,
    "TaskStatusId"  INT          NOT NULL,
    "AuthorId"      BIGINT       NOT NULL,
    "WatcherIds"    JSONB        NULL,
    "Name"          VARCHAR(255) NOT NULL,
    "Details"       TEXT         NULL,
    "Created"       TIMESTAMP    NOT NULL DEFAULT NOW(),
    "Updated"       TIMESTAMP    NOT NULL DEFAULT NOW(),
    "ProjectId"     BIGINT       NOT NULL,
    "ProjectTaskId" BIGINT       NOT NULL,
    "ResolutionId"  INT          NULL,
    "StatusId"      INT          NOT NULL,
    "TagIds"        JSONB        NULL,
    "TaskTypeId"    INT          NOT NULL,
    "ExecutorId"    BIGINT       NOT NULL,
    CONSTRAINT "PK_ProjectTasks_TaskId" PRIMARY KEY ("TaskId"),
    CONSTRAINT "FK_UserProjects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects"."UserProjects" ("ProjectId")
);

COMMENT ON TABLE "ProjectManagment"."ProjectTasks" IS 'Таблица задач проекта.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."TaskId" IS 'Id задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."TaskStatusId" IS 'Id статуса задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."AuthorId" IS 'Id пользователя, который является автором задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."WatcherIds" IS 'Id пользователей, которые являются наблюдателями задачи.
Jsonb в виде строки (например, 1,2,3).';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."Name" IS 'Название задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."Details" IS 'Описание задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."Created" IS 'Дата создания задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."Updated" IS 'Дата обновления задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."ProjectId" IS 'Id проекта, к которому принадлежит задача.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."ProjectTaskId" IS 'Id задачи, в рамках проекта. Нужен, чтобы нумерация Id задачи шло в рамках каждого проекта.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."ResolutionId" IS 'Id резолюции.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."TagIds" IS 'Список Id тегов задачи. В виде Jsonb.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."TaskTypeId" IS 'Id типа задачи.';
COMMENT ON COLUMN "ProjectManagment"."ProjectTasks"."ExecutorId" IS 'Id исполнителя задачи.';