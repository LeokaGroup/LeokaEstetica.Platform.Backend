DROP TABLE IF EXISTS project_management.task_links;

CREATE TABLE project_management.task_links
(
    link_id         BIGSERIAL
        CONSTRAINT pk_task_links_link_id
            PRIMARY KEY,
    from_task_id    BIGINT
        CONSTRAINT fk_project_tasks_from_task_id
            REFERENCES project_management.project_tasks,
    to_task_id      BIGINT
        CONSTRAINT fk_project_tasks_to_task_id
            REFERENCES project_management.project_tasks,
    link_type       project_management.LINK_TYPE_ENUM,
    parent_id       BIGINT
        CONSTRAINT fk_project_tasks_parent_id
            REFERENCES project_management.project_tasks,
    child_id        BIGINT
        CONSTRAINT fk_project_tasks_child_id
            REFERENCES project_management.project_tasks,
    is_blocked      BOOLEAN DEFAULT FALSE NOT NULL,
    project_id      BIGINT                NOT NULL
        CONSTRAINT fk_users_projects_project_id
            REFERENCES "Projects"."UserProjects",
    blocked_task_id BIGINT
);

COMMENT ON TABLE project_management.task_links IS 'Таблица связей между задачами.';

COMMENT ON COLUMN project_management.task_links.link_id IS 'PK.';

COMMENT ON COLUMN project_management.task_links.from_task_id IS 'Id задачи, которую связывают.';

COMMENT ON COLUMN project_management.task_links.to_task_id IS 'Id задачи, с которой связывают.';

COMMENT ON COLUMN project_management.task_links.link_type IS 'Тип связи.';

COMMENT ON COLUMN project_management.task_links.parent_id IS 'Id родителя.';

COMMENT ON COLUMN project_management.task_links.child_id IS 'Id дочки.';

COMMENT ON COLUMN project_management.task_links.is_blocked IS 'Признак блокирующей задачи.';

COMMENT ON COLUMN project_management.task_links.project_id IS 'Id проекта.';