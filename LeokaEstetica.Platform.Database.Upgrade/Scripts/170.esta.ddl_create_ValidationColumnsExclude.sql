CREATE TABLE "Configs"."ValidationColumnsExclude"
(
    "ValidationId" SERIAL
        CONSTRAINT "PK_ValidationColumnsExclude_ValidationId"
            PRIMARY KEY,
    "ParamName"    VARCHAR(150) NOT NULL
);