CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagmentTaskTemplates"
(
    "TemplateId"      SERIAL,
    "Position"        INT          NOT NULL DEFAULT 0,
    "TemplateName"    VARCHAR(100) NOT NULL,
    "TemplateSysName" VARCHAR(100) NOT NULL,
    CONSTRAINT "PK_ProjectManagmentTaskTemplates_TemplateId" PRIMARY KEY ("TemplateId")
);

COMMENT ON TABLE "Templates"."ProjectManagmentTaskTemplates" IS 'Таблица шаблонов задач. Содержит в себе шаблоны, которые касаются только задач (поддерживает и Kanban и Scrum).
По сути, это набор столбцов в рабочем пространстве. Каждый столбец - это отдельный статус линии задач (вертикальный столбец).';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskTemplates"."TemplateId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskTemplates"."Position" IS 'Порядковый номер.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskTemplates"."TemplateName" IS 'Название шаблона.';
COMMENT ON COLUMN "Templates"."ProjectManagmentTaskTemplates"."TemplateSysName" IS 'Системное название шаблона.';