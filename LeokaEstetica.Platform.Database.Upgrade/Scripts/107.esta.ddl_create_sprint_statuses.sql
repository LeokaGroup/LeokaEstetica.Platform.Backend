CREATE TABLE project_management.sprint_statuses
(
    status_id       SERIAL,
    status_name     VARCHAR(100) NOT NULL,
    status_sys_name VARCHAR(100) NOT NULL,
    CONSTRAINT pk_sprint_statuses_status_id PRIMARY KEY (status_id)
);

COMMENT ON TABLE project_management.sprint_statuses IS 'Таблица статусов спринтов.';
COMMENT ON COLUMN project_management.sprint_statuses.status_id IS 'PK.';
COMMENT ON COLUMN project_management.sprint_statuses.status_name IS 'Название статуса.';
COMMENT ON COLUMN project_management.sprint_statuses.status_sys_name IS 'Системное название статуса.';

CREATE UNIQUE INDEX uniq_sprint_statuses_status_name_idx ON project_management.sprint_statuses (status_name);