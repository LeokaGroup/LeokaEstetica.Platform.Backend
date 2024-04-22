create schema "Moderation";
create schema "Vacancies";

DROP TABLE IF EXISTS "Moderation"."Vacancies";

CREATE TABLE "Vacancies"."UserVacancies"
(
    "VacancyId"      BIGSERIAL    
        CONSTRAINT "PK_Vacancies_VacancyId"
            PRIMARY KEY
        CONSTRAINT "FK_UserVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies",
    "VacancyName"    VARCHAR(250)                                                                 NOT NULL,
    "VacancyText"    TEXT                                                                         NOT NULL,
    "WorkExperience" VARCHAR(100),
    "Employment"     VARCHAR(200),
    "DateCreated"    TIMESTAMP DEFAULT NOW()                                                      NOT NULL,
    "Payment"        VARCHAR(150),
    "UserId"         BIGINT                                                                       NOT NULL,
    "Conditions"     TEXT,
    "Demands"        TEXT
);

CREATE TABLE "Moderation"."ModerationStatuses"
(
    "StatusId"      SERIAL
        CONSTRAINT "PK_ModerationStatuses_StatusId"
            PRIMARY KEY,
    "StatusName"    VARCHAR(150) NOT NULL,
    "StatusSysName" VARCHAR(150) NOT NULL
);

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