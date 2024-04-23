CREATE TABLE "Knowledge"."KnowledgeSubCategoriesThemes"
(
    "SubCategoryThemeId"       BIGSERIAL
        CONSTRAINT "PK_KnowledgeSubCategories_SubCategoryThemeId"
            PRIMARY KEY,
    "SubCategoryThemeTitle"    VARCHAR(200)      NOT NULL,
    "SubCategoryThemeSubTitle" VARCHAR(300)      NOT NULL,
    "SubCategoryThemeText"     TEXT              NOT NULL,
    "SubCategoryThemeImg"      TEXT,
    "Position"                 INTEGER DEFAULT 0 NOT NULL,
    "SubCategoryId"            BIGINT
);