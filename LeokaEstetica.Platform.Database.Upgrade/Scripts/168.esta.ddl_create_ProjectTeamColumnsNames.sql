CREATE TABLE "Configs"."ProjectTeamColumnsNames"
(
    "ColumnId"   SERIAL
        CONSTRAINT "PK_ProjectTeamColumnsNames_ColumnId"
            PRIMARY KEY,
    "ColumnName" VARCHAR(200) NOT NULL,
    "TableName"  VARCHAR(200) NOT NULL,
    "Position"   INTEGER      NOT NULL
);