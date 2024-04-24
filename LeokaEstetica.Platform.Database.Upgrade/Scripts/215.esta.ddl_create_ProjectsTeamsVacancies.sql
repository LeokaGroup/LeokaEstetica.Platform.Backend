CREATE TABLE "Teams"."ProjectsTeamsVacancies"
(
    "TeamVacancyId" BIGSERIAL
        CONSTRAINT "PK_ProjectsTeamsVacancies_TeamVacancyId"
            PRIMARY KEY,
    "VacancyId"     BIGINT  NOT NULL,
    "IsActive"      BOOLEAN NOT NULL,
    "TeamId"        BIGINT  NOT NULL
);