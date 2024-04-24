CREATE TABLE "Communications"."MainInfoDialogs"
(
    "DialogId"   BIGSERIAL
        CONSTRAINT "PK_MainInfoDialogs_DialogId"
            PRIMARY KEY,
    "DialogName" VARCHAR(150)            NOT NULL,
    "Created"    TIMESTAMP DEFAULT NOW() NOT NULL,
    "ProjectId"  BIGINT
        CONSTRAINT "FK_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects",
    "VacancyId"  BIGINT
        CONSTRAINT "FK_UserVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies"
);