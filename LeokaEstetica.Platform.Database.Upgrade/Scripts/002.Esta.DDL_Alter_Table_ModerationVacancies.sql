DROP TABLE IF EXISTS "Moderation"."Vacancies";
CREATE TABLE IF NOT EXISTS "Moderation"."Vacancies"
(
    "ModerationId"       BIGSERIAL,
    "VacancyId"          BIGSERIAL
        CONSTRAINT "PK_Vacancies_ModerationId"
            PRIMARY KEY
        CONSTRAINT "FK_Vacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies",
    "DateModeration"     TIMESTAMP DEFAULT NOW() NOT NULL,
    "ModerationStatusId" INTEGER   DEFAULT 2     NOT NULL
        CONSTRAINT "FK_ModerationStatuses_StatusId"
            REFERENCES "Moderation"."ModerationStatuses"
);