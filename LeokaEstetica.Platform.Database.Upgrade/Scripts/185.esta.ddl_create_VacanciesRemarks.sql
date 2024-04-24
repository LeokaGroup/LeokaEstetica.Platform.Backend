CREATE TABLE "Moderation"."VacanciesRemarks"
(
    "RemarkId"         BIGSERIAL
        CONSTRAINT "PK_VacanciesRemarks_RemarkId"
            PRIMARY KEY,
    "VacancyId"        BIGINT
        CONSTRAINT "FK_Vacancies_UserVacancies_VacancyId"
            REFERENCES "Vacancies"."UserVacancies"
            ON DELETE CASCADE,
    "FieldName"        VARCHAR(100)                               NOT NULL,
    "RemarkText"       VARCHAR(500)                               NOT NULL,
    "RussianName"      VARCHAR(100)                               NOT NULL,
    "DateCreated"      TIMESTAMP    DEFAULT NOW()                 NOT NULL,
    "RemarkStatusId"   INTEGER
        CONSTRAINT "FK_Moderation_RemarksStatuses_RemarkStatusId"
            REFERENCES "Moderation"."RemarksStatuses",
    "ModerationUserId" BIGINT
        CONSTRAINT "FK_Users_UserId_ModerationUserId"
            REFERENCES dbo."Users",
    "RejectReason"     VARCHAR(300) DEFAULT ''::CHARACTER VARYING NOT NULL
);