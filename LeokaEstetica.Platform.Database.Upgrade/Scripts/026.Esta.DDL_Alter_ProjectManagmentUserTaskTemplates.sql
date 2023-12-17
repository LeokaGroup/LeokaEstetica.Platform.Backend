DROP TABLE IF EXISTS "Templates"."ProjectManagmentUserTaskTemplates";
CREATE TABLE IF NOT EXISTS "Templates"."ProjectManagmentUserTaskTemplates"
(
    "UserId"     BIGINT  NOT NULL,
    "TemplateId" INT     NOT NULL,
    "IsActive"   BOOLEAN NOT NULL,
    CONSTRAINT "PK_ProjectManagmentUserTaskTemplates_TemplateId_UserId" PRIMARY KEY ("UserId", "TemplateId"),
    CONSTRAINT "FK_Users_UserId" FOREIGN KEY ("UserId") REFERENCES dbo."Users" ("UserId")
);
COMMENT ON TABLE "Templates"."ProjectManagmentUserTaskTemplates" IS 'Таблица шаблонов, которые выбрал пользователь.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."UserId" IS 'Id пользователя.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."TemplateId" IS 'Id шаблона.';
COMMENT ON COLUMN "Templates"."ProjectManagmentUserTaskTemplates"."IsActive" IS 'Признак активности шаблона. Это поле нужно прежде всего для отображения всех шаблонов пользователя, что он выбирал ранее.';