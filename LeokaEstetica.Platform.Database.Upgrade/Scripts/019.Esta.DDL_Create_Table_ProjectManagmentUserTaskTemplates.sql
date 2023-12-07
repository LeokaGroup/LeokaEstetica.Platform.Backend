CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagmentUserTaskTemplates"
(
    "UserTemplateId" BIGSERIAL,
    "TemplateId"     INT,
    "IsActive"       BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_ProjectManagmentUserTaskTemplates_UserTemplateId" PRIMARY KEY ("UserTemplateId"),
    CONSTRAINT "FK_ProjectManagmentTaskTemplates_TemplateId" FOREIGN KEY ("TemplateId") REFERENCES "Templates"."ProjectManagmentTaskTemplates" ("TemplateId")
);
COMMENT ON TABLE "Templates"."ProjectManagmentUserTaskTemplates" IS 'Таблица шаблонов, которые выбрал пользователь.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."UserTemplateId" IS 'PK.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."TemplateId" IS 'Id шаблона.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."IsActive" IS 'Признак активности шаблона. Это поле нужно прежде всего для отображения всех шаблонов пользователя, что он выбирал ранее.';