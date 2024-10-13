CREATE TABLE communications.dialog_messages
(
    message_id    BIGSERIAL NOT NULL,
    dialog_id     BIGINT    NOT NULL,
    message       TEXT,
    created_at    TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by    BIGINT    NOT NULL,
    is_my_message BOOLEAN   NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_dialog_messages_message_id PRIMARY KEY (message_id),
    CONSTRAINT fk_main_info_dialogs_dialog_id FOREIGN KEY (dialog_id) REFERENCES communications.main_info_dialogs (dialog_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE communications.dialog_messages IS 'Таблица сообщений диалогов.';
COMMENT ON COLUMN communications.dialog_messages.message_id IS 'PK.';
COMMENT ON COLUMN communications.dialog_messages.dialog_id IS 'Id диалога.';
COMMENT ON COLUMN communications.dialog_messages.message IS 'Сообщение.
Может быть NULL, т.к. могут просто отправлять файл или документ и это не считается сообщением - это вложение.';
COMMENT ON COLUMN communications.dialog_messages.created_at IS 'Дата создания сообщения.';
COMMENT ON COLUMN communications.dialog_messages.created_by IS 'Id пользователя, который написал сообщение.';
COMMENT ON COLUMN communications.dialog_messages.is_my_message IS 'Признак принадлежности сообщения текущему пользователю.';