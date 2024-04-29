CREATE TABLE "Configs"."ProjectVacancyColumnsNames"
(
    "ColumnId"   SERIAL
        CONSTRAINT "PK_ProjectVacancyColumnsNames_ColumnId"
            PRIMARY KEY,
    "ColumnName" VARCHAR(200) NOT NULL,
    "TableName"  VARCHAR(200) NOT NULL,
    "Position"   INTEGER      NOT NULL
);