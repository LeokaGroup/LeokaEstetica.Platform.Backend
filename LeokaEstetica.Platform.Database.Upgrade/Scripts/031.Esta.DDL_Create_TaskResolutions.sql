CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskResolutions"
(
    "ResolutionId"      INT          NOT NULL,
    "ResolutionName"    VARCHAR(150) NOT NULL,
    "ResolutionSysName" VARCHAR(150) NOT NULL,
    "Position"          INT          NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TaskResolutions_ResolutionId" PRIMARY KEY ("ResolutionId")
);

COMMENT ON TABLE "ProjectManagment"."TaskResolutions" IS 'Таблица резолюций.';
COMMENT ON COLUMN "ProjectManagment"."TaskResolutions"."ResolutionId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskResolutions"."ResolutionName" IS 'Название резолюции.';
COMMENT ON COLUMN "ProjectManagment"."TaskResolutions"."ResolutionSysName" IS 'Системное название резолюции.';
COMMENT ON COLUMN "ProjectManagment"."TaskResolutions"."Position" IS 'Порядковый номер.';