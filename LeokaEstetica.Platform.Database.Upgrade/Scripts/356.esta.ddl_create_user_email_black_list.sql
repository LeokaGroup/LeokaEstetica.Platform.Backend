CREATE TABLE IF NOT EXISTS access.user_email_black_list
(
    black_id    BIGSERIAL                         NOT NULL,
    user_id     BIGINT                            NOT NULL,
    email       CHARACTER VARYING(120)            NOT NULL,
    CONSTRAINT pk_user_email_black_list_black_id PRIMARY KEY (black_id),
    CONSTRAINT fk_user_email_black_list_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE  access.user_email_black_list IS 'Таблица ЧС почты пользователей.';
COMMENT ON COLUMN access.user_email_black_list.black_id IS 'PK.';
COMMENT ON COLUMN access.user_email_black_list.user_id IS 'Id пользователя.';
COMMENT ON COLUMN access.user_email_black_list.email IS 'Email пользователя.';