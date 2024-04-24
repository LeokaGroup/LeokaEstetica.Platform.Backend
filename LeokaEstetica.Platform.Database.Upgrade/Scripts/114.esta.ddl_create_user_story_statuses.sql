CREATE TABLE project_management.user_story_statuses
(
    status_id       SERIAL,
    status_name     VARCHAR(100) NOT NULL,
    status_sys_name VARCHAR(100) NOT NULL,
    CONSTRAINT pk_user_story_statuses_status_id PRIMARY KEY (status_id)
);

COMMENT ON COLUMN project_management.user_story_statuses.status_id IS 'PK.';
COMMENT ON COLUMN project_management.user_story_statuses.status_name IS 'Название статуса.';
COMMENT ON COLUMN project_management.user_story_statuses.status_sys_name IS 'Системное название статуса.';