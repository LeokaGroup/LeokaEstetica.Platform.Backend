CREATE TABLE IF NOT EXISTS access.user_vk_black_list
(
    black_id        BIGSERIAL                         NOT NULL,
    user_id         BIGINT                            NOT NULL,
    vk_user_id      BIGINT                            NOT NULL,
    CONSTRAINT pk_user_vk_black_list_black_id PRIMARY KEY (black_id),
    CONSTRAINT fk_user_vk_black_list_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE  access.user_vk_black_list IS 'Таблица ЧС ВКонтакте пользователей.';
COMMENT ON COLUMN access.user_vk_black_list.black_id IS 'PK.';
COMMENT ON COLUMN access.user_vk_black_list.user_id IS 'Id пользователя.';
COMMENT ON COLUMN access.user_vk_black_list.vk_user_id IS 'Id пользователя в системе ВКонтакте.';