CREATE TABLE IF NOT EXISTS access.user_phone_black_list_history
(
    history_id          BIGSERIAL                               NOT NULL,
    date_created        timestamp without time zone             NOT NULL DEFAULT now(),
    action_text         CHARACTER VARYING(300)                  NOT NULL,
    action_sys_name     CHARACTER VARYING(100)                  NOT NULL,
    user_id             BIGINT                                  NOT NULL,
    phone_number        CHARACTER VARYING(50)                   NOT NULL,
    CONSTRAINT pk_user_phone_black_list_history_history_id PRIMARY KEY (history_id),
    CONSTRAINT fk_user_phone_black_list_history_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE  access.user_phone_black_list_history IS 'Таблица для хранения истории ЧС номеров телефонов пользователей.';
COMMENT ON COLUMN access.user_phone_black_list_history.history_id IS 'PK.';
COMMENT ON COLUMN access.user_phone_black_list_history.date_created IS 'Дата создания записи истории.';
COMMENT ON COLUMN access.user_phone_black_list_history.action_text IS 'Текст события.';
COMMENT ON COLUMN access.user_phone_black_list_history.action_sys_name IS 'Системное название события.';
COMMENT ON COLUMN access.user_phone_black_list_history.user_id IS 'Id пользователя.';
COMMENT ON COLUMN access.user_phone_black_list_history.phone_number IS 'Номер телефона пользователя.';