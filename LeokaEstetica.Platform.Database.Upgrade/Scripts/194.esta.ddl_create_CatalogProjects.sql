CREATE TABLE "Projects"."CatalogProjects"
(
    "CatalogProjectId" BIGSERIAL
        CONSTRAINT "PK_CatalogProjects_CatalogProjectId"
            PRIMARY KEY,
    "ProjectId"        BIGINT
        CONSTRAINT "Uniq_CatalogProjects_ProjectId"
            UNIQUE
        CONSTRAINT "FK_UserProjects_ProjectId"
            REFERENCES "Projects"."UserProjects"
            ON DELETE CASCADE
);