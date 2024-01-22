ALTER TABLE IF EXISTS templates."ProjectManagementTransitionIntermediateTemplates"
    RENAME TO project_management_transition_intermediate_templates;

ALTER TABLE IF EXISTS templates.project_management_transition_intermediate_templates
    RENAME COLUMN "TransitionId" TO transition_id;

ALTER TABLE IF EXISTS templates.project_management_transition_intermediate_templates
    RENAME COLUMN "FromStatusId" TO from_status_id;

ALTER TABLE IF EXISTS templates.project_management_transition_intermediate_templates
    RENAME COLUMN "ToStatusId" TO to_status_id;

ALTER TABLE IF EXISTS templates.project_management_transition_intermediate_templates
    RENAME COLUMN "IsCustomTransition" TO is_custom_transition;

ALTER TABLE IF EXISTS templates.project_management_transition_intermediate_templates
    RENAME CONSTRAINT "PK_ProjectManagementTransitionIntermediateTemplates_TransitionI" TO pk_transition_intermediate_templates_transition_id;