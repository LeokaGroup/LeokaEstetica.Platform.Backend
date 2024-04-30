CREATE TABLE settings.project_user_strategy
(
    strategy_id SERIAL,
    project_id  BIGINT                NOT NULL,
    user_id     BIGINT                NOT NULL,
    strategy    settings.project_strategy_enum NOT NULL,
    CONSTRAINT pk_project_user_strategy_strategy_id PRIMARY KEY (strategy_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId"),
    CONSTRAINT fk_users_user_id FOREIGN KEY (user_id) REFERENCES dbo."Users" ("UserId")
);