CREATE TABLE project_management.epic_tasks
(
    epic_tasks_id   BIGSERIAL,
    project_task_id BIGINT NOT NULL,
    epic_id         BIGINT NOT NULL,
    CONSTRAINT pk_epic_tasks_epic_tasks_id PRIMARY KEY (epic_tasks_id),
    CONSTRAINT fk_project_task_id FOREIGN KEY (project_task_id) REFERENCES project_management.project_tasks (project_task_id),
    CONSTRAINT fk_epics_epic_id FOREIGN KEY (epic_id) REFERENCES project_management.epics (epic_id)
);

COMMENT ON TABLE project_management.epic_tasks IS 'Таблица задач эпиков.';
COMMENT ON COLUMN project_management.epic_tasks.epic_tasks_id IS 'PK.';
COMMENT ON COLUMN project_management.epic_tasks.project_task_id IS 'Id задачи в рамках проекта.';
COMMENT ON COLUMN project_management.epic_tasks.epic_id IS 'Id епика.';

CREATE UNIQUE INDEX IF NOT EXISTS uniq_epic_tasks_project_task_id_epic_id_idx ON project_management.epic_tasks (project_task_id, epic_id);