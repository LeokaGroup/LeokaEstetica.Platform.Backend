DROP TABLE dbo.landing_menu;
CREATE TABLE dbo.landing_menu
(
    menu_id SERIAL NOT NULL,
    items   JSONB,
    CONSTRAINT pk_landing_menu_menu_id PRIMARY KEY (menu_id)
);

COMMENT ON TABLE dbo.landing_menu IS 'Таблица landing меню.';
COMMENT ON COLUMN dbo.landing_menu.menu_id IS 'PK.';
COMMENT ON COLUMN dbo.landing_menu.items IS 'Элементы меню.';