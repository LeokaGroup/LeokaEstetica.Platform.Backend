CREATE TABLE settings.project_user_strategy
(
    strategy_id SERIAL
        CONSTRAINT pk_project_user_strategy_strategy_id
            PRIMARY KEY,
    project_id  BIGINT                         NOT NULL
        CONSTRAINT fk_user_projects_project_id
            REFERENCES "Projects"."UserProjects",
    user_id     BIGINT                         NOT NULL
        CONSTRAINT fk_users_user_id
            REFERENCES dbo."Users",
    strategy    settings.project_strategy_enum NOT NULL
);