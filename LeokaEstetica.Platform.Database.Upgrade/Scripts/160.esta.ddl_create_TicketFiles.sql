CREATE TABLE "Communications"."TicketFiles"
(
    "FileId"      BIGSERIAL
        CONSTRAINT "PK_TicketFiles_FileId"
            PRIMARY KEY,
    "Url"         TEXT                    NOT NULL,
    "Title"       VARCHAR(150),
    "Description" TEXT,
    "Position"    SMALLINT                NOT NULL,
    "Type"        "TicketFileTypeEnum"    NOT NULL,
    "DateCreated" TIMESTAMP DEFAULT NOW() NOT NULL
);