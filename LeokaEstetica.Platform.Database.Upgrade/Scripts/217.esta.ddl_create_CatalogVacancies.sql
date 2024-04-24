CREATE TABLE "Vacancies"."CatalogVacancies"
(
    "CatalogVacancyId" BIGSERIAL
        CONSTRAINT "PK_CatalogVacancies_CatalogVacancyId"
            PRIMARY KEY,
    "VacancyId"        BIGINT NOT NULL
        CONSTRAINT "Uniq_CatalogVacancies_VacancyId"
            UNIQUE
        CONSTRAINT "FK_CatalogVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies"
            ON DELETE CASCADE
);