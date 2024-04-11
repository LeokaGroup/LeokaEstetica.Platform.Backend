ALTER TABLE project_management.epics
    ADD COLUMN project_epic_id BIGINT NOT NULL;

CREATE INDEX idx_project_tasks_project_id_project_epic_id
    ON project_management.epics (project_id, project_epic_id);

CREATE UNIQUE INDEX uniq_idx_project_tasks_project_epic_id
    ON project_management.epics (project_epic_id);