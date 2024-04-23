CREATE TABLE "Knowledge"."KnowledgeStart"
(
    "StartId"             BIGSERIAL
        CONSTRAINT "PK_KnowledgeStart_StartId"
            PRIMARY KEY,
    "CategoryTitle"       VARCHAR(200)                NOT NULL,
    "CategoryTypeName"    VARCHAR(200)                NOT NULL,
    "CategoryTypeSysName" VARCHAR(200)                NOT NULL,
    "SubCategoryTitle"    VARCHAR(200)                NOT NULL,
    "Position"            INTEGER DEFAULT 0           NOT NULL,
    "TopCategories"       JSONB   DEFAULT '[]'::JSONB NOT NULL
);