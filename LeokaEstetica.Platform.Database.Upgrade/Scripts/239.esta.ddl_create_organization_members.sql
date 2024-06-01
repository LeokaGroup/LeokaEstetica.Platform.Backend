CREATE TABLE project_management.organization_members
(
    organization_member_id       BIGSERIAL,
    organization_id BIGINT                   NOT NULL,
    member_role     VARCHAR(200)             NULL,
    member_id       BIGINT                   NOT NULL,
    joined          TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_organization_members_member_id PRIMARY KEY (organization_member_id),
    CONSTRAINT fk_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES project_management.organizations (organization_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (member_id) REFERENCES dbo."Users" ("UserId")
);