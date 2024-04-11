ALTER TABLE project_management.epics
    ADD COLUMN status_id INT NOT NULL DEFAULT 1;

ALTER TABLE project_management.epics
    ADD CONSTRAINT fk_epic_statuses_status_id FOREIGN KEY (status_id) REFERENCES project_management.epic_statuses (status_id);