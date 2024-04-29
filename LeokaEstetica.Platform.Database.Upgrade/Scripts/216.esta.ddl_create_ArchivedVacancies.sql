CREATE TABLE "Vacancies"."ArchivedVacancies"
(
    "ArchiveId"    BIGSERIAL
        CONSTRAINT "PK_ArchivedVacancies_ArchiveId"
            PRIMARY KEY,
    "VacancyId"    BIGINT                  NOT NULL
        CONSTRAINT "FK_UserVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies"
            ON DELETE CASCADE,
    "DateArchived" TIMESTAMP DEFAULT NOW() NOT NULL,
    "UserId"       BIGINT                  NOT NULL
);