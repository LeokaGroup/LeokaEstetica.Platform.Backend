CREATE TABLE project_management.epic_statuses
(
    status_id       SERIAL
        CONSTRAINT pk_epic_statuses_status_id
            PRIMARY KEY,
    status_name     VARCHAR(100) NOT NULL,
    status_sys_name VARCHAR(100) NOT NULL
);

COMMENT ON COLUMN project_management.epic_statuses.status_id IS 'PK.';
COMMENT ON COLUMN project_management.epic_statuses.status_name IS 'Название статуса.';
COMMENT ON COLUMN project_management.epic_statuses.status_sys_name IS 'Системное название статуса.';