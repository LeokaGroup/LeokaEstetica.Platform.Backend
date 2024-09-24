CREATE TABLE roles.component_user_roles
(
    component_user_role_id SERIAL NOT NULL,
    component_role_id      INT    NOT NULL,
    user_id                BIGINT NOT NULL,
    CONSTRAINT pk_component_user_roles_component_user_role_id PRIMARY KEY (component_user_role_id),
    CONSTRAINT fk_component_roles_component_role_id FOREIGN KEY (component_role_id) REFERENCES roles.component_roles (component_role_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE roles.component_user_roles IS 'Таблица компонентных ролей пользователей.
Компонентные роли не связаны с функциональными ролями пользователей (ролевой моделью).';
COMMENT ON COLUMN roles.component_user_roles.component_user_role_id IS 'PK';
COMMENT ON COLUMN roles.component_user_roles.component_role_id IS 'Id компонентной роли.';
COMMENT ON COLUMN roles.component_user_roles.user_id IS 'Id пользователя';
