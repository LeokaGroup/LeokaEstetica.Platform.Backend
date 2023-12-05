CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskStatuses"
(
    "StatusId"      INT          NOT NULL,
    "StatusName"    VARCHAR(150) NOT NULL,
    "StatusSysName" VARCHAR(150) NOT NULL,
    "Position"      INT          NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TaskStatuses_StatusId" PRIMARY KEY ("StatusId")
);

COMMENT ON TABLE "ProjectManagment"."TaskStatuses" IS 'Таблица статусов задач.';
COMMENT ON COLUMN "ProjectManagment"."TaskStatuses"."StatusId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskStatuses"."StatusName" IS 'Название статуса.';
COMMENT ON COLUMN "ProjectManagment"."TaskStatuses"."StatusSysName" IS 'Системное название статуса.';
COMMENT ON COLUMN "ProjectManagment"."TaskStatuses"."Position" IS 'Порядковый номер.';