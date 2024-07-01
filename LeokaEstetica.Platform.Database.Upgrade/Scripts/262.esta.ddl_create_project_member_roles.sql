CREATE TABLE roles.project_member_roles
(
    project_member_role_id BIGSERIAL NOT NULL,
    role_id                BIGINT    NOT NULL,
    project_member_id      BIGINT    NOT NULL,
    organization_id        BIGINT    NOT NULL,
    is_enabled             BOOLEAN   NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_project_member_roles_project_member_role_id PRIMARY KEY (project_member_role_id),
    CONSTRAINT fk_project_roles_role_id FOREIGN KEY (role_id) REFERENCES roles.project_roles (role_id),
    CONSTRAINT fk_project_management_organizations FOREIGN KEY (organization_id) REFERENCES project_management.organizations (organization_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (project_member_id) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE roles.project_member_roles IS 'Таблица ролей участника проекта.';
COMMENT ON COLUMN roles.project_member_roles.project_member_role_id IS 'PK.';
COMMENT ON COLUMN roles.project_member_roles.role_id IS 'Id роли участника проекта.';
COMMENT ON COLUMN roles.project_member_roles.project_member_id IS 'Id участника роли проекта.';
COMMENT ON COLUMN roles.project_member_roles.organization_id IS 'Id организации.';
COMMENT ON COLUMN roles.project_member_roles.is_enabled IS 'Признак активной роли.';