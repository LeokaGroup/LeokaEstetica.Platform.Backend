CREATE TABLE "Knowledge"."KnowledgeSubCategories"
(
    "SubCategoryId"          BIGSERIAL
        CONSTRAINT "PK_KnowledgeSubCategories_SubCategoryId"
            PRIMARY KEY,
    "CategoryId"             BIGINT
        CONSTRAINT "FK_KnowledgeCategories_CategoryId"
            REFERENCES "Knowledge"."KnowledgeCategories",
    "Position"               INTEGER DEFAULT 0 NOT NULL,
    "SubCategoryTypeName"    VARCHAR(200)      NOT NULL,
    "SubCategoryTypeSysName" VARCHAR(200)      NOT NULL,
    "SubCategoryThemeId"     BIGINT
        CONSTRAINT "FK_KnowledgeSubCategoriesThemes_SubCategoryThemeId"
            REFERENCES "Knowledge"."KnowledgeSubCategoriesThemes"
);