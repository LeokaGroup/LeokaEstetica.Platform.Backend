CREATE TABLE project_management.organization_projects
(
    organization_project_id BIGSERIAL,
    organization_id         BIGINT  NOT NULL,
    project_id              BIGINT  NOT NULL,
    is_active               BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_organization_projects_organization_project_id PRIMARY KEY (organization_project_id),
    CONSTRAINT fk_organizations_organization_id FOREIGN KEY (organization_id) REFERENCES project_management.organizations (organization_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId")
);