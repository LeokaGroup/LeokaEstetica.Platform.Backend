CREATE TABLE "Configs"."ProjectColumnsNames"
(
    "ColumnId"   BIGSERIAL
        CONSTRAINT "PK_ColumnsNames_ColumnId"
            PRIMARY KEY,
    "ColumnName" VARCHAR(200) NOT NULL,
    "TableName"  VARCHAR(200) NOT NULL,
    "Position"   INTEGER      NOT NULL
);