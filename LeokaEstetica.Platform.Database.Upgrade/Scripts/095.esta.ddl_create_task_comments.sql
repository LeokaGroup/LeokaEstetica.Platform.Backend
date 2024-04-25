DROP TABLE IF EXISTS project_management.task_comments;

CREATE TABLE project_management.task_comments
(
    comment_id      BIGSERIAL,
    project_id      BIGINT    NOT NULL,
    project_task_id BIGINT    NOT NULL,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP NULL,
    comment         TEXT      NOT NULL,
    created_by      BIGINT    NOT NULL,
    updated_by      BIGINT    NULL,
    CONSTRAINT pk_task_comments_comment_id PRIMARY KEY (comment_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId"),
    CONSTRAINT fk_project_tasks_task_id FOREIGN KEY (project_task_id) REFERENCES project_management.project_tasks (task_id),
    CONSTRAINT fk_users_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId"),
    CONSTRAINT fk_users_updated_by FOREIGN KEY (updated_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE project_management.task_comments IS 'Таблица комментариев задачи.';
COMMENT ON COLUMN project_management.task_comments.comment_id IS 'PK. Id комментария.';
COMMENT ON COLUMN project_management.task_comments.project_id IS 'Id проекта.';
COMMENT ON COLUMN project_management.task_comments.project_task_id IS 'Id задачи, которой принадлежит комментарий.';
COMMENT ON COLUMN project_management.task_comments.created_at IS 'Дата создания комментария.';
COMMENT ON COLUMN project_management.task_comments.updated_at IS 'Дата изменения комментария.';
COMMENT ON COLUMN project_management.task_comments.comment IS 'Текст комментария.';
COMMENT ON COLUMN project_management.task_comments.created_by IS 'Id пользователя создавшего комментарий.';
COMMENT ON COLUMN project_management.task_comments.updated_by IS 'Id пользователя изменившего комментарий.';