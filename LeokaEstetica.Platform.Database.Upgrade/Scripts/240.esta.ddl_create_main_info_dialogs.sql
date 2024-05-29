CREATE TABLE ai.scrum_master_ai_main_info_dialogs
(
    dialog_id   BIGSERIAL,
    dialog_name VARCHAR(150)             NOT NULL,
    created     TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    object_id   BIGINT                   NULL,
    object_type ai.object_type_dialog_ai    NOT NULL DEFAULT 'scrum_master_ai',
    CONSTRAINT pk_scrum_master_ai_main_info_dialogs_dialog_id PRIMARY KEY (dialog_id)
);