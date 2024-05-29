CREATE TABLE ai.scrum_master_ai_dialog_members
(
    member_id BIGSERIAL                NOT NULL,
    joined    TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    dialog_id BIGINT                   NOT NULL,
    user_id   BIGINT                   NOT NULL,
    CONSTRAINT pk_scrum_master_ai_dialog_members_member_id PRIMARY KEY (member_id),
    CONSTRAINT fk_scrum_master_ai_main_info_dialogs_dialog_id FOREIGN KEY (dialog_id) REFERENCES ai.scrum_master_ai_main_info_dialogs (dialog_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);