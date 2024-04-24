CREATE TABLE project_management.sprint_tasks
(
    sprint_task_id BIGSERIAL,
    sprint_id BIGINT NOT NULL,
    project_task_id BIGINT NOT NULL,
    CONSTRAINT pk_sprint_tasks_sprint_task_id PRIMARY KEY (sprint_task_id)
);

COMMENT ON TABLE project_management.sprint_tasks IS 'Таблица задач, которые включены в спринты.';
COMMENT ON COLUMN project_management.sprint_tasks.sprint_task_id IS 'PK.';
COMMENT ON COLUMN project_management.sprint_tasks.sprint_id IS 'Id спринта, в который включена задача.';
COMMENT ON COLUMN project_management.sprint_tasks.project_task_id IS 'Id задачи, которая включена в спринт.';

CREATE UNIQUE INDEX IF NOT EXISTS uniq_sprint_statuses_status_name_idx ON project_management.sprint_tasks (sprint_id, project_task_id);