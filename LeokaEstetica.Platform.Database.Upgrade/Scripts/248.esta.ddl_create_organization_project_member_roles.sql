CREATE SCHEMA roles;

CREATE TABLE roles.organization_project_member_roles
(
    role_id                BIGSERIAL,
    organization_id        BIGINT       NOT NULL,
    organization_member_id BIGINT       NOT NULL,
    role_name              VARCHAR(200) NOT NULL,
    role_sys_name          VARCHAR(200) NOT NULL,
    is_active              BOOLEAN      NOT NULL DEFAULT TRUE,
    is_enabled             BOOLEAN      NOT NULL DEFAULT FALSE,
    project_id             BIGINT       NULL,
    CONSTRAINT pk_organization_project_member_roles_role_id PRIMARY KEY (role_id)
);

COMMENT ON TABLE roles.organization_project_member_roles IS 'Таблица ролей участников проекта компании.';
COMMENT ON COLUMN roles.organization_project_member_roles.role_id IS 'PK.';
COMMENT ON COLUMN roles.organization_project_member_roles.organization_id IS 'Id компании.';
COMMENT ON COLUMN roles.organization_project_member_roles.organization_member_id IS 'Id участника компании.';
COMMENT ON COLUMN roles.organization_project_member_roles.role_name IS 'Название роли.';
COMMENT ON COLUMN roles.organization_project_member_roles.role_sys_name IS 'Системное название роли.';
COMMENT ON COLUMN roles.organization_project_member_roles.is_active IS 'Признак активной роли.';
COMMENT ON COLUMN roles.organization_project_member_roles.is_enabled IS 'Признак активной роли у участника проекта компании.';
COMMENT ON COLUMN roles.organization_project_member_roles.project_id IS 'Id проекта компании.';