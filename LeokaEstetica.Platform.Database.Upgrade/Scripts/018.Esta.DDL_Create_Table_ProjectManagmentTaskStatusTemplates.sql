CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagmentTaskStatusTemplates"
(
    "StatusId"      SERIAL,
    "StatusName"    VARCHAR(100) NOT NULL,
    "StatusSysName" VARCHAR(100) NOT NULL,
    "Position"      INT          NOT NULL DEFAULT 0,
    "TemplateId"    INT          NOT NULL,
    CONSTRAINT "PK_ProjectManagmentTaskStatusTemplates_StatusId" PRIMARY KEY ("StatusId"),
    CONSTRAINT "FK_ProjectManagmentTaskTemplates_TemplateId" FOREIGN KEY ("TemplateId") REFERENCES "Templates"."ProjectManagmentTaskTemplates" ("TemplateId")
);
COMMENT ON TABLE "Templates"."ProjectManagmentTaskStatusTemplates" IS 'Таблица шаблонов статусов задач.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskStatusTemplates"."StatusId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskStatusTemplates"."StatusName" IS 'Название статуса.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskStatusTemplates"."StatusSysName" IS 'Системное название статуса.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskStatusTemplates"."Position" IS 'Порядковый номер.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskStatusTemplates"."TemplateId" IS 'Id шаблона.';