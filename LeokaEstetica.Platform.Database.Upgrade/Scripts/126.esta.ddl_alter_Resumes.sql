create schema "Profile";

CREATE TABLE "Profile"."ProfilesInfo"
(
    "ProfileInfoId"    BIGSERIAL
        CONSTRAINT "PK_ProfilesInfo_ProfileInfoId"
            PRIMARY KEY,
    "LastName"         VARCHAR(200) NOT NULL,
    "FirstName"        VARCHAR(200) NOT NULL,
    "Patronymic"       VARCHAR(200),
    "IsShortFirstName" BOOLEAN      NOT NULL,
    "Telegram"         VARCHAR(200),
    "WhatsApp"         VARCHAR(200),
    "Vkontakte"        VARCHAR(200),
    "OtherLink"        VARCHAR(200),
    "Aboutme"          TEXT         NOT NULL,
    "Job"              VARCHAR(200),
    "UserId"           BIGINT
        CONSTRAINT "FK_Users_UserId"
            REFERENCES dbo."Users"
            ON DELETE CASCADE,
    "WorkExperience"   VARCHAR(50)
);

CREATE TABLE "Moderation"."Resumes"
(
    "ModerationId"       BIGSERIAL
        CONSTRAINT "PK_Resumes_ModerationId"
            PRIMARY KEY,
    "ProfileInfoId"      BIGINT                                 NOT NULL
        CONSTRAINT "FK_ProfilesInfo_ProfileInfoId"
            REFERENCES "Profile"."ProfilesInfo",
    "DateModeration"     TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    "ModerationStatusId" INTEGER                                NOT NULL
        CONSTRAINT "FK_ModerationStatuses_StatusId"
            REFERENCES "Moderation"."ModerationStatuses"
);

ALTER TABLE "Moderation"."Resumes"
    ALTER COLUMN "DateModeration" TYPE TIMESTAMP WITH TIME ZONE;