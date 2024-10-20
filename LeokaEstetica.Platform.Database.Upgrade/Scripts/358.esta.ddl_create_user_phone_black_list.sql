CREATE TABLE IF NOT EXISTS access.user_phone_black_list
(
    black_id        BIGSERIAL                         NOT NULL,
    user_id         BIGINT                            NOT NULL,
    phone_number    CHARACTER VARYING(50)             NOT NULL,
    CONSTRAINT pk_user_phone_black_list_black_id PRIMARY KEY (black_id),
    CONSTRAINT fk_user_phone_black_list_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE  access.user_phone_black_list IS 'Таблица ЧС номеров телефонов пользователей.';
COMMENT ON COLUMN access.user_phone_black_list.black_id IS 'PK.';
COMMENT ON COLUMN access.user_phone_black_list.user_id IS 'Id пользователя.';
COMMENT ON COLUMN access.user_phone_black_list.phone_number IS 'Номер телефона пользователя.';