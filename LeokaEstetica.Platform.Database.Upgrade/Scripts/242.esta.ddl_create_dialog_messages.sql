CREATE TABLE ai.scrum_master_ai_dialog_messages
(
    message_id    BIGSERIAL                NOT NULL,
    message       TEXT                     NOT NULL,
    created       TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    dialog_id     BIGINT                   NOT NULL,
    user_id       BIGINT                   NOT NULL,
    is_my_message BOOLEAN                  NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_scrum_master_ai_dialog_messages_message_id PRIMARY KEY (message_id),
    CONSTRAINT fk_scrum_master_ai_main_info_dialogs_dialog_id FOREIGN KEY (dialog_id) REFERENCES ai.scrum_master_ai_main_info_dialogs (dialog_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);