CREATE TABLE dbo.left_menu
(
    menu_id SERIAL NOT NULL,
    items   JSONB,
    CONSTRAINT pk_left_menu_menu_id PRIMARY KEY (menu_id)
);

COMMENT ON TABLE dbo.left_menu IS 'Таблица левого меню.';
COMMENT ON COLUMN dbo.left_menu.menu_id IS 'PK.';
COMMENT ON COLUMN dbo.left_menu.items IS 'Элементы меню.';