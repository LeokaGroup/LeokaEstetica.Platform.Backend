ALTER TABLE project_management.sprints
    ADD COLUMN sprint_name VARCHAR(200) NOT NULL;

COMMENT ON COLUMN project_management.sprints.sprint_name IS 'Название спринта.';