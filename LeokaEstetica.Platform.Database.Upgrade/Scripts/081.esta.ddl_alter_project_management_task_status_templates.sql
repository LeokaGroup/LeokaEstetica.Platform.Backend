ALTER TABLE IF EXISTS templates."ProjectManagmentTaskStatusTemplates"
    RENAME TO project_management_task_status_templates;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME COLUMN "StatusId" TO status_id;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME COLUMN "StatusName" TO status_name;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME COLUMN "StatusSysName" TO status_sys_name;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME COLUMN "Position" TO position;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME COLUMN "TaskStatusId" TO task_status_id;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME COLUMN "StatusDescription" TO status_description;

ALTER TABLE IF EXISTS templates.project_management_task_status_templates
    RENAME CONSTRAINT "PK_ProjectManagmentTaskStatusTemplates_StatusId" TO pk_project_management_task_status_templates_status_id;