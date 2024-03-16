ALTER TABLE project_management.user_stories
    ADD COLUMN status_id INT NOT NULL DEFAULT 1;

ALTER TABLE project_management.user_stories
    ADD CONSTRAINT fk_user_stories_status_id FOREIGN KEY (status_id) REFERENCES project_management.user_story_statuses (status_id);