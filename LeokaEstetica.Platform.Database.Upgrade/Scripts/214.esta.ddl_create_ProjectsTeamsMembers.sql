CREATE TABLE "Teams"."ProjectsTeamsMembers"
(
    "MemberId"  BIGSERIAL
        CONSTRAINT "PK_ProjectsTeamsMembers_MemberId"
            PRIMARY KEY,
    "VacancyId" BIGINT,
    "Joined"    TIMESTAMP NOT NULL,
    "UserId"    BIGINT    NOT NULL
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users",
    "TeamId"    BIGINT    NOT NULL
        CONSTRAINT "FK_ProjectsTeams_TeamId"
            REFERENCES "Teams"."ProjectsTeams"
            ON DELETE CASCADE
);