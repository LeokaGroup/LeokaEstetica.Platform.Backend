CREATE TABLE communications.group_object_actions_menu
(
    menu_id SERIAL NOT NULL,
    items   JSONB,
    CONSTRAINT pk_left_menu_menu_id PRIMARY KEY (menu_id)
);

COMMENT ON TABLE communications.group_object_actions_menu IS 'Таблица меню действий групп объектов.';
COMMENT ON COLUMN communications.group_object_actions_menu.menu_id IS 'PK.';
COMMENT ON COLUMN communications.group_object_actions_menu.items IS 'Элементы меню.';