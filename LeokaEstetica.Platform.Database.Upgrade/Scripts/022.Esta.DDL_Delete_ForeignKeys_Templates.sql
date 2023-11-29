ALTER TABLE IF EXISTS "Templates"."ProjectManagmentTaskStatusTemplates"
    DROP CONSTRAINT IF EXISTS "FK_ProjectManagmentTaskTemplates_TemplateId";

ALTER TABLE IF EXISTS "Templates"."ProjectManagmentUserTaskTemplates"
    DROP CONSTRAINT IF EXISTS "FK_ProjectManagmentTaskTemplates_TemplateId";