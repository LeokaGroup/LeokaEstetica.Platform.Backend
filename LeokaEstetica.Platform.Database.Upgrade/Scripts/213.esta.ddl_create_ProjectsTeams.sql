CREATE TABLE "Teams"."ProjectsTeams"
(
    "TeamId"    BIGSERIAL
        CONSTRAINT "PK_ProjectsTeams_TeamId"
            PRIMARY KEY,
    "ProjectId" BIGINT    NOT NULL
        UNIQUE
        CONSTRAINT "FK_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects",
    "Created"   TIMESTAMP NOT NULL
);