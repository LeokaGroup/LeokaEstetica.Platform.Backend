ALTER TABLE templates.project_management_task_status_templates
    ADD COLUMN is_system_status BOOLEAN NOT NULL DEFAULT FALSE;