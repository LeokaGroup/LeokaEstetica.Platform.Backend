CREATE TABLE dbo.top_menu
(
    menu_id SERIAL NOT NULL,
    items   JSONB,
    CONSTRAINT pk_left_menu_menu_id PRIMARY KEY (menu_id)
);

COMMENT ON TABLE dbo.top_menu IS 'Таблица верхнего меню.';
COMMENT ON COLUMN dbo.top_menu.menu_id IS 'PK.';
COMMENT ON COLUMN dbo.top_menu.items IS 'Элементы меню.';