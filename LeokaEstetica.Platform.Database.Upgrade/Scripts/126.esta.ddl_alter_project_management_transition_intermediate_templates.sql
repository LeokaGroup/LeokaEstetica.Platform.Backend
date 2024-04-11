ALTER TABLE templates.project_management_transition_intermediate_templates
    ADD COLUMN transition_type templates.transition_type_enum DEFAULT 'task';