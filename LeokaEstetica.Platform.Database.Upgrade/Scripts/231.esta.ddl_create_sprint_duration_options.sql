CREATE TABLE settings.sprint_duration_settings
(
    setting_id  BIGSERIAL,
    name       VARCHAR(400) NOT NULL,
    sys_name   VARCHAR(150) NULL,
    tooltip    VARCHAR(400) NULL,
    selected   BOOLEAN      NOT NULL DEFAULT FALSE,
    disabled   BOOLEAN      NOT NULL DEFAULT FALSE,
    project_id BIGINT       NOT NULL,
    user_id    BIGINT       NOT NULL,
    CONSTRAINT pk_sprint_duration_settings_id PRIMARY KEY (setting_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId"),
    CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE settings.sprint_duration_settings IS 'Таблица Scrum настроек проекта.';
COMMENT ON COLUMN settings.sprint_duration_settings.setting_id IS 'PK.';
COMMENT ON COLUMN settings.sprint_duration_settings.name IS 'Название настройки.';
COMMENT ON COLUMN settings.sprint_duration_settings.sys_name IS 'Системное название настройки.';
COMMENT ON COLUMN settings.sprint_duration_settings.tooltip IS 'Текст подсказки к настройке.';
COMMENT ON COLUMN settings.sprint_duration_settings.selected IS 'Признак выбранной пользователем настройки.';
COMMENT ON COLUMN settings.sprint_duration_settings.disabled IS 'Признак заблокированной настройки.';
COMMENT ON COLUMN settings.sprint_duration_settings.project_id IS 'Id проекта.';
COMMENT ON COLUMN settings.sprint_duration_settings.user_id IS 'Id пользователя.';