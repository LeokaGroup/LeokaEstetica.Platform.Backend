CREATE TABLE project_management.wiki_context_menu
(
    menu_id       SERIAL       NOT NULL,
    item_name     VARCHAR(150) NOT NULL,
    icon          VARCHAR(150) NULL,
    item_sys_name VARCHAR(150) NULL
);

COMMENT ON TABLE project_management.wiki_context_menu IS 'Таблица контекстного меню дерева wiki.';
COMMENT ON COLUMN project_management.wiki_context_menu.menu_id IS 'PK.';
COMMENT ON COLUMN project_management.wiki_context_menu.item_name IS 'Название элемента меню.';
COMMENT ON COLUMN project_management.wiki_context_menu.icon IS 'Иконка элемента меню.';
COMMENT ON COLUMN project_management.wiki_context_menu.item_sys_name IS 'Системное название элемента меню.';