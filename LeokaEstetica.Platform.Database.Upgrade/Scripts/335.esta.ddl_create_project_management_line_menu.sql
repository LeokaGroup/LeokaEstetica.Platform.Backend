CREATE TABLE project_management.project_management_line_menu
(
    menu_id SERIAL NOT NULL,
    items   JSONB,
    CONSTRAINT pk_project_management_line_menu_menu_id PRIMARY KEY (menu_id)
);

COMMENT ON TABLE project_management.project_management_line_menu IS 'Таблица меню для блока быстрых действий в раб.пространстве проекта.';
COMMENT ON COLUMN project_management.project_management_line_menu.menu_id IS 'PK.';
COMMENT ON COLUMN project_management.project_management_line_menu.items IS 'Элементы меню.';