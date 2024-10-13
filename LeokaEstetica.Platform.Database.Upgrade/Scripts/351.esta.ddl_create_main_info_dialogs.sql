CREATE TABLE communications.main_info_dialogs
(
    dialog_id         BIGSERIAL    NOT NULL,
    dialog_name       VARCHAR(255) NOT NULL,
    created_at        TIMESTAMP       NOT NULL DEFAULT NOW(),
    abstract_scope_id BIGINT,
    CONSTRAINT pk_main_info_dialogs_dialog_id PRIMARY KEY (dialog_id)
);

COMMENT ON TABLE communications.main_info_dialogs IS 'Таблица с основной информацией о диалогах.';
COMMENT ON COLUMN communications.main_info_dialogs.dialog_id IS 'PK.';
COMMENT ON COLUMN communications.main_info_dialogs.dialog_name IS 'Название диалога.';
COMMENT ON COLUMN communications.main_info_dialogs.created_at IS 'Дата создания диалога.';
COMMENT ON COLUMN communications.main_info_dialogs.abstract_scope_id IS 'Id группы абстрактной области чата (если есть).
Если нету, то это диалог между участниками без привязки к проекту и тд.';