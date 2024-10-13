CREATE TABLE communications.dialog_members
(
    member_id BIGSERIAL NOT NULL,
    user_id   BIGINT    NOT NULL,
    joined    TIMESTAMP NOT NULL DEFAULT NOW(),
    dialog_id BIGINT    NOT NULL,
    CONSTRAINT pk_dialog_members_member_id PRIMARY KEY (member_id),
    CONSTRAINT fk_main_info_dialogs_dialog_id FOREIGN KEY (dialog_id) REFERENCES communications.main_info_dialogs (dialog_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE communications.dialog_members IS 'Таблица участников диалога.';
COMMENT ON COLUMN communications.dialog_members.member_id IS 'PK.';
COMMENT ON COLUMN communications.dialog_members.user_id IS 'Id пользователя.';
COMMENT ON COLUMN communications.dialog_members.joined IS 'Дата присоединения к диалогу.';
COMMENT ON COLUMN communications.dialog_members.dialog_id IS 'Id диалога.';