CREATE TABLE access.access_module
(
    module_id       SERIAL                         NOT NULL,
    module_name     VARCHAR(150)                   NOT NULL,
    module_sys_name VARCHAR(150)                   NOT NULL,
    is_access       BOOLEAN                        NOT NULL DEFAULT FALSE,
    module_type     access.ACCESS_MODULE_TYPE_ENUM NOT NULL,
    CONSTRAINT pk_access_module_module_id PRIMARY KEY (module_id)
);

COMMENT ON TABLE access.access_module IS 'Таблица доступов к модулям платформы.';
COMMENT ON COLUMN access.access_module.module_id IS 'PK.';
COMMENT ON COLUMN access.access_module.module_name IS 'Название модуля, доступ к которому проверяется. Например какой то компонент модуля.';
COMMENT ON COLUMN access.access_module.module_sys_name IS 'Системное название модуля, доступ к которому проверяется. Например проект, вакансия.';
COMMENT ON COLUMN access.access_module.is_access IS 'Признак наличия доступа к модулю.';
COMMENT ON COLUMN access.access_module.module_type IS 'Тип модуля.';

CREATE UNIQUE INDEX access_module_module_name_module_sys_name_module_type_idx ON access.access_module (module_name, module_sys_name, module_type);