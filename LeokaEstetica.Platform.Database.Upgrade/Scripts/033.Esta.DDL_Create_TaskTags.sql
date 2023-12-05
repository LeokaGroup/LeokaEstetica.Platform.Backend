CREATE TABLE IF NOT EXISTS "ProjectManagment"."TaskTags"
(
    "TagId"      INT          NOT NULL,
    "TagName"    VARCHAR(150) NOT NULL,
    "TagSysName" VARCHAR(150) NOT NULL,
    "Position"   INT          NOT NULL DEFAULT 0,
    CONSTRAINT "PK_TaskTags_TagId" PRIMARY KEY ("TagId")
);

COMMENT ON TABLE "ProjectManagment"."TaskTags" IS 'Таблица тегов задачи.';
COMMENT ON COLUMN "ProjectManagment"."TaskTags"."TagId" IS 'PK.';
COMMENT ON COLUMN "ProjectManagment"."TaskTags"."TagName" IS 'Название тега.';
COMMENT ON COLUMN "ProjectManagment"."TaskTags"."TagSysName" IS 'Системное название тега.';
COMMENT ON COLUMN "ProjectManagment"."TaskTags"."Position" IS 'Порядковый номер.';