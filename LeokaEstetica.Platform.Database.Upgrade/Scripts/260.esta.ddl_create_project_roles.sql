DROP TABLE roles.organization_project_member_roles;

CREATE TABLE roles.project_roles
(
    role_id       BIGSERIAL    NOT NULL,
    role_name     VARCHAR(200) NOT NULL,
    role_sys_name VARCHAR(200) NOT NULL,
    CONSTRAINT pk_project_roles_role_id PRIMARY KEY (role_id)
);

COMMENT ON TABLE roles.project_roles IS 'Таблица ролей проекта.';
COMMENT ON COLUMN roles.project_roles.role_id IS 'PK.';
COMMENT ON COLUMN roles.project_roles.role_name IS 'Название роли.';
COMMENT ON COLUMN roles.project_roles.role_sys_name IS 'Системное название роли.';