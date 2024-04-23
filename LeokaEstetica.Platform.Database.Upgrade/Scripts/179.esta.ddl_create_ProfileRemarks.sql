CREATE TABLE "Moderation"."ProfileRemarks"
(
    "RemarkId"         BIGSERIAL
        CONSTRAINT "PK_ProfileRemarks_RemarkId"
            PRIMARY KEY,
    "ProfileInfoId"    BIGINT
        CONSTRAINT "FK_Profile_ProfileInfoId"
            REFERENCES "Profile"."ProfilesInfo",
    "FieldName"        VARCHAR(100)                               NOT NULL,
    "RemarkText"       VARCHAR(500)                               NOT NULL,
    "RussianName"      VARCHAR(100)                               NOT NULL,
    "ModerationUserId" BIGINT
        CONSTRAINT "FK_Users_UserId_ModerationUserId"
            REFERENCES dbo."Users",
    "DateCreated"      TIMESTAMP    DEFAULT NOW()                 NOT NULL,
    "RemarkStatusId"   INTEGER
        CONSTRAINT "FK_Moderation_RemarksStatuses_RemarkStatusId"
            REFERENCES "Moderation"."RemarksStatuses",
    "RejectReason"     VARCHAR(300) DEFAULT ''::CHARACTER VARYING NOT NULL
);