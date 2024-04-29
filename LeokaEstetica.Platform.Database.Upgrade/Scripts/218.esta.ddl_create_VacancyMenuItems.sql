CREATE TABLE "Vacancies"."VacancyMenuItems"
(
    "VacancyMenuItemId" SERIAL
        CONSTRAINT "PK_VacancyMenuItems_VacancyMenuItemId"
            PRIMARY KEY,
    "VacancyMenuItems"  JSONB NOT NULL
);