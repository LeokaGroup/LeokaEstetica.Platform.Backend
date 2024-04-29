CREATE TABLE "Rules"."DiscountRules"
(
    "RuleId"      SERIAL
        CONSTRAINT "PK_DiscountRules_RuleId"
            PRIMARY KEY,
    "Percent"     NUMERIC(6, 4) DEFAULT 0 NOT NULL,
    "Type"        VARCHAR(150)            NOT NULL,
    "RussianName" VARCHAR(150)            NOT NULL,
    "Month"       SMALLINT
        CONSTRAINT "DiscountRules_Check_Month"
            CHECK (("Month" > 0) AND ("Month" <= 12))
);