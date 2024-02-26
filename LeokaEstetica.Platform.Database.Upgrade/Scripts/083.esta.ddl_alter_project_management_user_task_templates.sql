ALTER TABLE IF EXISTS templates."ProjectManagmentUserTaskTemplates"
    RENAME TO project_management_user_task_templates;

ALTER TABLE IF EXISTS templates.project_management_user_task_templates
    RENAME COLUMN "UserId" TO user_id;

ALTER TABLE IF EXISTS templates.project_management_user_task_templates
    RENAME COLUMN "TemplateId" TO template_id;

ALTER TABLE IF EXISTS templates.project_management_user_task_templates
    RENAME COLUMN "IsActive" TO is_active;

ALTER TABLE IF EXISTS templates.project_management_user_task_templates
    RENAME CONSTRAINT "PK_ProjectManagmentUserTaskTemplates_TemplateId_UserId" TO pk_project_management_user_task_templates_template_id_user_id;