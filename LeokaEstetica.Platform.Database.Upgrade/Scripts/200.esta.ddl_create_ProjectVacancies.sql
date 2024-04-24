CREATE TABLE "Projects"."ProjectVacancies"
(
    "ProjectVacancyId" BIGSERIAL
        CONSTRAINT "PK_ProjectVacancies_ProjectVacancyId"
            PRIMARY KEY,
    "ProjectId"        BIGINT NOT NULL,
    "VacancyId"        BIGINT
        CONSTRAINT "FK_UserVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies"
            ON DELETE CASCADE
);