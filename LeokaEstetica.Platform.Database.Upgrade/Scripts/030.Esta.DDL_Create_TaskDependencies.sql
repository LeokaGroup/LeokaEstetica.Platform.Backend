CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskDependencies"
(
    "DependencyId"       BIGINT       NOT NULL,
    "TaskId"             BIGINT       NOT NULL,
    "DependencySysType"  VARCHAR(150) NOT NULL,
    "DependencyTypeName" VARCHAR(150) NOT NULL,
    "Position"           INT          NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TaskDependencies_DependencyId" PRIMARY KEY ("DependencyId"),
    CONSTRAINT "FK_UserTasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "ProjectManagment"."UserTasks" ("TaskId")
);

COMMENT ON TABLE "ProjectManagment"."TaskDependencies" IS 'Таблица зависимостей между задачами (Блокирует/блокируется, Клонирует/клонируется, Дублирует/дублируется, Связано с).';
COMMENT ON COLUMN "ProjectManagment"."TaskDependencies"."DependencyId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskDependencies"."TaskId" IS 'Id задачи.';
COMMENT ON COLUMN "ProjectManagment"."TaskDependencies"."DependencySysType" IS 'Системное название типа зависимости.';
COMMENT ON COLUMN "ProjectManagment"."TaskDependencies"."DependencyTypeName" IS 'Название типа зависимости. (например, Блокирует/блокируется, Клонирует/клонируется, Дублирует/дублируется, Связано с).';
COMMENT ON COLUMN "ProjectManagment"."TaskDependencies"."Position" IS 'Порядковый номер.';