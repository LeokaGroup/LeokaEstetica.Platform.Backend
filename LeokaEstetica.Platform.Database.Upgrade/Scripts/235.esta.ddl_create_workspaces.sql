CREATE TABLE project_management.workspaces
(
    workspace_id BIGSERIAL,
    project_id   BIGINT NOT NULL,
    CONSTRAINT pk_workspaces_workspace_id PRIMARY KEY (workspace_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId")
);