CREATE TABLE project_management.epics
(
    epic_id          BIGSERIAL,
    epic_name        VARCHAR(250) NOT NULL,
    epic_description TEXT         NULL,
    created_by       BIGINT       NOT NULL,
    created_at       TIMESTAMP    NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMP    NULL,
    updated_by       BIGINT       NULL,
    project_task_id  BIGINT       NOT NULL,
    project_id       BIGINT       NOT NULL,
    initiative_id    BIGINT       NULL,
    CONSTRAINT pk_epic_id PRIMARY KEY (epic_id),
    CONSTRAINT fk_users_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId"),
    CONSTRAINT fk_users_updated_by FOREIGN KEY (updated_by) REFERENCES dbo."Users" ("UserId"),
    CONSTRAINT fk_project_tasks_project_task_id FOREIGN KEY (project_task_id) REFERENCES project_management.project_tasks (project_task_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId")
);

COMMENT ON TABLE project_management.epics IS 'Таблица эпиков.';
COMMENT ON COLUMN project_management.epics.epic_id IS 'PK.';
COMMENT ON COLUMN project_management.epics.epic_name IS 'Название эпика.';
COMMENT ON COLUMN project_management.epics.epic_description IS 'Описание эпика.';
COMMENT ON COLUMN project_management.epics.created_by IS 'Пользователь, который создал эпик.';
COMMENT ON COLUMN project_management.epics.created_at IS 'Дата создания эпика.';
COMMENT ON COLUMN project_management.epics.updated_at IS 'Дата обновления эпика.';
COMMENT ON COLUMN project_management.epics.updated_by IS 'Пользователь, который обновил эпик.';
COMMENT ON COLUMN project_management.epics.project_task_id IS 'Id задачи в рамках проекта, которая включена в эпик.';
COMMENT ON COLUMN project_management.epics.project_id IS 'Id проекта.';
COMMENT ON COLUMN project_management.epics.initiative_id IS 'Id инициативы, включающая в себя эпики.';