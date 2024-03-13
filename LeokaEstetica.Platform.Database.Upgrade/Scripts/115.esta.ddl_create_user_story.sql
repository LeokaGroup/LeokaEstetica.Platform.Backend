CREATE TABLE project_management.user_story
(
    story_id          BIGSERIAL,
    story_name        VARCHAR(250) NOT NULL,
    story_description TEXT,
    created_by        BIGINT       NOT NULL,
    created_at        TIMESTAMP    NOT NULL DEFAULT NOW(),
    updated_at        TIMESTAMP,
    updated_by        BIGINT       NOT NULL,
    project_id        BIGINT       NOT NULL,
    story_status_id   INT          NOT NULL,
    watcher_ids       BIGINT[],
    resolution_id     INT,
    tag_ids           INT[],
    epic_id           BIGINT,
    executor_id       BIGINT,
    CONSTRAINT pk_user_story_story_id PRIMARY KEY (story_id),
    CONSTRAINT fk_epics_epic_id FOREIGN KEY (epic_id) REFERENCES project_management.epics (epic_id),
    CONSTRAINT fk_users_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId"),
    CONSTRAINT fk_users_updated_by FOREIGN KEY (updated_by) REFERENCES dbo."Users" ("UserId"),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId"),
    CONSTRAINT fk_user_story_statuses_status_id FOREIGN KEY (story_status_id) REFERENCES project_management.user_story_statuses (status_id)
);

COMMENT ON COLUMN project_management.user_story.story_id IS 'PK.';
COMMENT ON COLUMN project_management.user_story.story_name IS 'Название истории/требования.';
COMMENT ON COLUMN project_management.user_story.story_description IS 'Описание истории/требования.';
COMMENT ON COLUMN project_management.user_story.story_description IS 'Описание истории/требования.';
COMMENT ON COLUMN project_management.user_story.created_by IS 'Пользователь, который создал историю/требование.';
COMMENT ON COLUMN project_management.user_story.created_at IS 'Дата создания истории/требования.';
COMMENT ON COLUMN project_management.user_story.updated_at IS 'Дата обновления истории/требования.';
COMMENT ON COLUMN project_management.user_story.updated_by IS 'Пользователь, который обновил историю/требование.';
COMMENT ON COLUMN project_management.user_story.project_id IS 'Id проекта.';
COMMENT ON COLUMN project_management.user_story.story_status_id IS 'Статус истории/требования.';
COMMENT ON COLUMN project_management.user_story.watcher_ids IS 'Наблюдатели истории/требования.';
COMMENT ON COLUMN project_management.user_story.resolution_id IS 'Id резолюции.';
COMMENT ON COLUMN project_management.user_story.tag_ids IS 'Теги истории/требования.';
COMMENT ON COLUMN project_management.user_story.epic_id IS 'Id эпика, к которому относится история/требование.';
COMMENT ON COLUMN project_management.user_story.executor_id IS 'Id исполнителя.';