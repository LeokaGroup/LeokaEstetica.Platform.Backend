ALTER TABLE IF EXISTS templates."ProjectManagmentTaskStatusIntermediateTemplates"
    RENAME TO project_management_task_status_intermediate_templates;

ALTER TABLE IF EXISTS templates.project_management_task_status_intermediate_templates
    RENAME COLUMN "StatusId" TO status_id;

ALTER TABLE IF EXISTS templates.project_management_task_status_intermediate_templates
    RENAME COLUMN "TemplateId" TO template_id;

ALTER TABLE IF EXISTS templates.project_management_task_status_intermediate_templates
    RENAME COLUMN "IsCustomStatus" TO is_custom_status;

ALTER TABLE IF EXISTS templates.project_management_task_status_intermediate_templates
    RENAME CONSTRAINT "PK_ProjectManagmentTaskStatusTemplates_StatusId_TemplateId" TO pk_task_status_templates_status_id_template_id;