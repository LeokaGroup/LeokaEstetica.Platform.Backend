ALTER TABLE IF EXISTS templates."ProjectManagmentTaskTemplates"
    RENAME TO project_management_task_templates;

ALTER TABLE IF EXISTS templates.project_management_task_templates
    RENAME COLUMN "TemplateId" TO template_id;

ALTER TABLE IF EXISTS templates.project_management_task_templates
    RENAME COLUMN "Position" TO position;

ALTER TABLE IF EXISTS templates.project_management_task_templates
    RENAME COLUMN "TemplateName" TO template_name;

ALTER TABLE IF EXISTS templates.project_management_task_templates
    RENAME COLUMN "TemplateSysName" TO template_sys_name;

ALTER TABLE IF EXISTS templates.project_management_task_templates
    RENAME CONSTRAINT "PK_ProjectManagmentTaskTemplates_TemplateId" TO pk_project_management_task_templates_template_id;