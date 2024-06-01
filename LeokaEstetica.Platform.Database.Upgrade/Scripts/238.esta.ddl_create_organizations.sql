CREATE TABLE project_management.organizations
(
    organization_id   BIGSERIAL,
    organization_name VARCHAR(200)             NULL,
    created_by        BIGINT                   NOT NULL,
    created_at        TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_organizations_organization_id PRIMARY KEY (organization_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);