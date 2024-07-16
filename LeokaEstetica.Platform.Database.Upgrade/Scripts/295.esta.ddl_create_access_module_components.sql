CREATE TABLE access.access_module_components
(
    component_id       SERIAL       NOT NULL,
    object_id          BIGINT       NOT NULL,
    component_name     VARCHAR(150) NOT NULL,
    component_sys_name VARCHAR(150) NOT NULL,
    is_access          BOOLEAN      NOT NULL DEFAULT FALSE,
    module_id          INT          NOT NULL,
    CONSTRAINT pk_access_module_components_component_id PRIMARY KEY (component_id),
    CONSTRAINT fk_access_module_module_id FOREIGN KEY (module_id) REFERENCES access.access_module (module_id)
);

COMMENT ON TABLE access.access_module_components IS 'Таблица компонентов доступов к модулям платформы.';
COMMENT ON COLUMN access.access_module_components.component_id IS 'PK.';
COMMENT ON COLUMN access.access_module_components.object_id IS 'Id объекта, доступ к которому проверяется. Например проект, вакансия.';
COMMENT ON COLUMN access.access_module_components.component_name IS 'Название компонента модуля, доступ к которому проверяется. Например какой то компонент модуля.';
COMMENT ON COLUMN access.access_module_components.component_sys_name IS 'Системное название компонента модуля, доступ к которому проверяется. Например проект, вакансия.';
COMMENT ON COLUMN access.access_module_components.is_access IS 'Признак наличия доступа к модулю.';

CREATE UNIQUE INDEX component_id_object_id_component_name_component_sys_name_module_id ON access.access_module_components (component_id, object_id, component_name, component_sys_name, module_id);