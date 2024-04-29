CREATE TABLE "Vacancies"."VacancyStatuses"
(
    "StatusId"             SERIAL
        CONSTRAINT "PK_VacancyStatuses_StatusId"
            PRIMARY KEY,
    "VacancyStatusSysName" VARCHAR(100) NOT NULL,
    "VacancyStatusName"    VARCHAR(100) NOT NULL,
    "VacancyId"            INTEGER
        CONSTRAINT "FK_UserVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies"
            ON DELETE CASCADE
);