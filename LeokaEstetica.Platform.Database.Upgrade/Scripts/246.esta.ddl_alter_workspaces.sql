ALTER TABLE project_management.workspaces
    ADD COLUMN organization_id BIGINT NOT NULL,
    ADD CONSTRAINT fk_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES project_management.organizations (organization_id);