CREATE TABLE "Knowledge"."KnowledgeCategories"
(
    "CategoryId"          BIGSERIAL
        CONSTRAINT "PK_KnowledgeCategories_CategoryId"
            PRIMARY KEY,
    "CategoryTitle"       VARCHAR(200)          NOT NULL,
    "SubCategoryId"       BIGINT                NOT NULL,
    "SubCategoryTypeName" VARCHAR(200)          NOT NULL,
    "CategoryTypeSysName" VARCHAR(200)          NOT NULL,
    "IsTop"               BOOLEAN DEFAULT FALSE NOT NULL,
    "Position"            INTEGER DEFAULT 0     NOT NULL,
    "StartId"             BIGINT
        CONSTRAINT "FK_KnowledgeStart_StartId"
            REFERENCES "Knowledge"."KnowledgeStart"
);