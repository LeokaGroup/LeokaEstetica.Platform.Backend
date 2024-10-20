CREATE TABLE IF NOT EXISTS access.user_vk_black_list_history
(
    history_id          BIGSERIAL                         NOT NULL,
    date_created        timestamp without time zone       NOT NULL DEFAULT now(),
    action_text         CHARACTER VARYING(300)            NOT NULL,
    action_sys_name     CHARACTER VARYING(100)            NOT NULL,
    user_id             BIGINT                            NOT NULL,
    vk_user_id          BIGINT                            NOT NULL,
    CONSTRAINT pk_user_vk_black_list_history_history_id PRIMARY KEY (history_id),
    CONSTRAINT fk_user_vk_black_list_history_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE  access.user_vk_black_list_history IS 'Таблица для хранения истории ЧС ВКонтакте пользователей.';
COMMENT ON COLUMN access.user_vk_black_list_history.history_id IS 'PK.';
COMMENT ON COLUMN access.user_vk_black_list_history.date_created IS 'Дата создания записи истории.';
COMMENT ON COLUMN access.user_vk_black_list_history.action_text IS 'Текст события.';
COMMENT ON COLUMN access.user_vk_black_list_history.action_sys_name IS 'Системное название события.';
COMMENT ON COLUMN access.user_vk_black_list_history.user_id IS 'Id пользователя.';
COMMENT ON COLUMN access.user_vk_black_list_history.vk_user_id IS 'Id пользователя в системе ВКонтакте.';