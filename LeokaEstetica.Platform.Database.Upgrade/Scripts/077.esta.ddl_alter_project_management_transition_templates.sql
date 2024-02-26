ALTER TABLE IF EXISTS templates."ProjectManagementTransitionTemplates"
    RENAME TO project_management_transition_templates;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME COLUMN "TransitionId" TO transition_id;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME COLUMN "TransitionName" TO transition_name;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME COLUMN "TransitionSysName" TO transition_sys_name;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME COLUMN "Position" TO position;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME COLUMN "FromStatusId" TO from_status_id;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME COLUMN "ToStatusId" TO to_status_id;

ALTER TABLE IF EXISTS templates.project_management_transition_templates
    RENAME CONSTRAINT "PK_ProjectManagementTransitionTemplates_TransitionId" TO pk_project_management_transition_templates_transition_id;