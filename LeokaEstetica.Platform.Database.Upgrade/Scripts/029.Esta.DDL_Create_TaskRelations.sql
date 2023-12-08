CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskRelations"
(
    "RelationId"   BIGSERIAL       NOT NULL,
    "RelationType" VARCHAR(150) NOT NULL,
    "TaskId"       BIGINT       NOT NULL,
    CONSTRAINT "PK_TaskRelations_RelationId" PRIMARY KEY ("RelationId", "TaskId"),
    CONSTRAINT "FK_UserTasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "ProjectManagment"."ProjectTasks" ("TaskId")
);

COMMENT ON TABLE "ProjectManagment"."TaskRelations" IS 'Таблица отношений между задачами (родитель, дочка).';
COMMENT ON COLUMN "ProjectManagment"."TaskRelations"."RelationId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskRelations"."RelationType" IS 'Тип отношения (дочка, родитель).';
COMMENT ON COLUMN "ProjectManagment"."TaskRelations"."TaskId" IS 'Id задачи.';