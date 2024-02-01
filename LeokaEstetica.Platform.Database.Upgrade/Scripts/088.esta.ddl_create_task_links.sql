CREATE TABLE project_management.task_links
(
    link_id    BIGSERIAL,
    task_id    BIGINT  NOT NULL,
    link_type  project_management.LINK_TYPE_ENUM,
    parent_id  BIGINT  NULL,
    child_id   BIGINT  NULL,
    is_blocked BOOLEAN NOT NULL DEFAULT FALSE,
    project_id BIGINT  NOT NULL,
    CONSTRAINT pk_task_links_link_id PRIMARY KEY (link_id),
    CONSTRAINT fk_project_tasks_project_task_id FOREIGN KEY (task_id) REFERENCES project_management.project_tasks (task_id),
    CONSTRAINT fk_project_tasks_parent_id FOREIGN KEY (parent_id) REFERENCES project_management.project_tasks (task_id),
    CONSTRAINT fk_project_tasks_child_id FOREIGN KEY (child_id) REFERENCES project_management.project_tasks (task_id),
    CONSTRAINT fk_users_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId")
);

COMMENT ON TABLE project_management.task_links IS 'Таблица связей между задачами.';
COMMENT ON COLUMN project_management.task_links.link_id IS 'PK.';
COMMENT ON COLUMN project_management.task_links.task_id IS 'Id задачи.';
COMMENT ON COLUMN project_management.task_links.link_type IS 'Тип связи.';
COMMENT ON COLUMN project_management.task_links.parent_id IS 'Id родителя.';
COMMENT ON COLUMN project_management.task_links.child_id IS 'Id дочки.';
COMMENT ON COLUMN project_management.task_links.is_blocked IS 'Признак блокирующей задачи.';
COMMENT ON COLUMN project_management.task_links.project_id IS 'Id проекта.';