DROP TABLE IF EXISTS project_management.user_stories;

CREATE TABLE project_management.user_stories
(
    story_id           BIGSERIAL
        CONSTRAINT pk_user_story_story_id
            PRIMARY KEY,
    story_name         VARCHAR(250)            NOT NULL,
    story_description  TEXT,
    created_by         BIGINT                  NOT NULL
        CONSTRAINT fk_users_created_by
            REFERENCES dbo."Users",
    created_at         TIMESTAMP DEFAULT NOW() NOT NULL,
    updated_at         TIMESTAMP,
    updated_by         BIGINT
        CONSTRAINT fk_users_updated_by
            REFERENCES dbo."Users",
    project_id         BIGINT                  NOT NULL
        CONSTRAINT fk_user_projects_project_id
            REFERENCES "Projects"."UserProjects",
    story_status_id    INTEGER                 NOT NULL
        CONSTRAINT fk_user_story_statuses_status_id
            REFERENCES project_management.user_story_statuses,
    watcher_ids        BIGINT[],
    resolution_id      INTEGER,
    tag_ids            INTEGER[],
    epic_id            BIGINT
        CONSTRAINT fk_epics_epic_id
            REFERENCES project_management.epics,
    executor_id        BIGINT,
    user_story_task_id BIGINT                  NOT NULL,
    status_id          INTEGER   DEFAULT 1     NOT NULL
        CONSTRAINT fk_user_stories_status_id
            REFERENCES project_management.user_story_statuses
);

COMMENT ON COLUMN project_management.user_stories.story_id IS 'PK.';

COMMENT ON COLUMN project_management.user_stories.story_name IS 'Название истории/требования.';

COMMENT ON COLUMN project_management.user_stories.story_description IS 'Описание истории/требования.';

COMMENT ON COLUMN project_management.user_stories.created_by IS 'Пользователь, который создал историю/требование.';

COMMENT ON COLUMN project_management.user_stories.created_at IS 'Дата создания истории/требования.';

COMMENT ON COLUMN project_management.user_stories.updated_at IS 'Дата обновления истории/требования.';

COMMENT ON COLUMN project_management.user_stories.updated_by IS 'Пользователь, который обновил историю/требование.';

COMMENT ON COLUMN project_management.user_stories.project_id IS 'Id проекта.';

COMMENT ON COLUMN project_management.user_stories.story_status_id IS 'Статус истории/требования.';

COMMENT ON COLUMN project_management.user_stories.watcher_ids IS 'Наблюдатели истории/требования.';

COMMENT ON COLUMN project_management.user_stories.resolution_id IS 'Id резолюции.';

COMMENT ON COLUMN project_management.user_stories.tag_ids IS 'Теги истории/требования.';

COMMENT ON COLUMN project_management.user_stories.epic_id IS 'Id эпика, к которому относится история/требование.';

COMMENT ON COLUMN project_management.user_stories.executor_id IS 'Id исполнителя.';

CREATE INDEX idx_user_stories_project_id_user_story_task_id
    ON project_management.user_stories (project_id, user_story_task_id);

CREATE UNIQUE INDEX uniq_idx_user_stories_user_story_task_id
    ON project_management.user_stories (user_story_task_id);