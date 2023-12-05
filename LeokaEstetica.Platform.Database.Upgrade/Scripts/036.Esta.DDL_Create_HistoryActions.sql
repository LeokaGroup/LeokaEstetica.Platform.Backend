CREATE TABLE IF NOT EXISTS "ProjectManagment"."HistoryActions"
(
    "ActionId"      INT          NOT NULL,
    "ActionName"    VARCHAR(150) NOT NULL,
    "ActionSysName" VARCHAR(150) NOT NULL,
    "Position"      INT          NOT NULL DEFAULT 0,
    CONSTRAINT "PK_HistoryActions_ActionId" PRIMARY KEY ("ActionId")
);

COMMENT ON TABLE "ProjectManagment"."HistoryActions" IS 'Таблица действий над задачей для истории действий.';
COMMENT ON COLUMN "ProjectManagment"."HistoryActions"."ActionId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."HistoryActions"."ActionName" IS 'Название действия.';
COMMENT ON COLUMN "ProjectManagment"."HistoryActions"."ActionSysName" IS 'Системное название действия.';
COMMENT ON COLUMN "ProjectManagment"."HistoryActions"."Position" IS 'Порядковый номер.';