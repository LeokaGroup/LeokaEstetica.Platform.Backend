ALTER TABLE project_management.user_stories
DROP CONSTRAINT fk_epics_epic_id;

ALTER TABLE project_management.user_stories
    ADD CONSTRAINT fk_epics_epic_id FOREIGN KEY (epic_id) REFERENCES project_management.epics (epic_id) ON DELETE CASCADE;