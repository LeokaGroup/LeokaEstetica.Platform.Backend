ALTER TABLE project_management.user_task_tags
    ADD COLUMN project_id BIGINT;

ALTER TABLE project_management.user_task_tags
    DROP COLUMN user_id;

ALTER TABLE project_management.user_task_tags
    ADD CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId");

ALTER TABLE project_management.user_task_tags
    RENAME TO project_tags;