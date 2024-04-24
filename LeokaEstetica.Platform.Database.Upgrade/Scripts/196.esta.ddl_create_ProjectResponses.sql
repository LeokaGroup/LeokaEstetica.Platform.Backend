CREATE TABLE "Projects"."ProjectResponses"
(
    "ResponseId"               BIGSERIAL
        CONSTRAINT "PK_ProjectResponses_ResponseId"
            PRIMARY KEY,
    "ProjectId"                BIGINT                  NOT NULL,
    "UserId"                   BIGINT                  NOT NULL,
    "DateResponse"             TIMESTAMP DEFAULT NOW() NOT NULL,
    "ProjectResponseStatuseId" INTEGER                 NOT NULL,
    "VacancyId"                BIGINT
);