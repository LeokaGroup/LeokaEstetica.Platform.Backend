ALTER TABLE documents.project_documents
    DROP CONSTRAINT fk_user_projects_task_id;

ALTER TABLE documents.project_documents
    ADD CONSTRAINT fk_project_tasks_project_task_id FOREIGN KEY (task_id) REFERENCES project_management.project_tasks (project_task_id);