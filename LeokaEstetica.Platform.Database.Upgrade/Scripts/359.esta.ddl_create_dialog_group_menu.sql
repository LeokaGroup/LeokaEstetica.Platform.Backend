CREATE TABLE communications.dialog_group_menu
(
    menu_id SERIAL NOT NULL,
    items   JSONB,
    CONSTRAINT pk_dialog_group_menu_id PRIMARY KEY (menu_id)
);

COMMENT ON TABLE communications.dialog_group_menu IS 'Таблица меню группировки диалогов.';
COMMENT ON COLUMN communications.dialog_group_menu.menu_id IS 'PK.';
COMMENT ON COLUMN communications.dialog_group_menu.items IS 'Элементы меню.';