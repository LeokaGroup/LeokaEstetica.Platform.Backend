ALTER TABLE IF EXISTS templates."ProjectManagementUserStatuseTemplates"
    RENAME TO project_management_user_statuse_templates;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME COLUMN "StatusId" TO status_id;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME COLUMN "StatusName" TO status_name;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME COLUMN "StatusSysName" TO status_sys_name;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME COLUMN "Position" TO position;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME COLUMN "UserId" TO user_id;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME COLUMN "StatusDescription" TO status_description;

ALTER TABLE IF EXISTS templates.project_management_user_statuse_templates
    RENAME CONSTRAINT "PK_ProjectManagementUserStatuseTemplates_StatusId" TO pk_project_management_user_statuse_templates_status_id;