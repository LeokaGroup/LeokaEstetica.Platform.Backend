CREATE TABLE "Projects"."ArchivedProjects"
(
    "ArchiveId"    BIGSERIAL
        CONSTRAINT "PK_CatalogProjects_ArchiveId"
            PRIMARY KEY,
    "DateArchived" TIMESTAMP DEFAULT NOW() NOT NULL,
    "ProjectId"    BIGINT                  NOT NULL
        CONSTRAINT "FK_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects"
            ON DELETE CASCADE,
    "UserId"       BIGINT                  NOT NULL
);