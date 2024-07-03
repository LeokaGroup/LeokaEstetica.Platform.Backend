ALTER TABLE project_management.epic_tasks
DROP CONSTRAINT IF EXISTS fk_epics_project_epic_id;

ALTER TABLE project_management.epic_tasks
    ADD CONSTRAINT fk_epics_project_epic_id FOREIGN KEY (epic_id) REFERENCES project_management.epics (epic_id) ON DELETE CASCADE;