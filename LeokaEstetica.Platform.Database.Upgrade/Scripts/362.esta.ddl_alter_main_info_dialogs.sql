ALTER TABLE communications.main_info_dialogs
    ADD COLUMN dialog_group_type communications.dialog_group_type_enum NOT NULL DEFAULT 'project';

COMMENT ON COLUMN communications.main_info_dialogs.dialog_group_type IS 'Тип группировки диалогов.';