CREATE TABLE "Commerce"."Products"
(
    "ProductId"       BIGSERIAL
        CONSTRAINT "PK_Products_ProductId"
            PRIMARY KEY,
    "ProductName"     VARCHAR(150)          NOT NULL,
    "IsDiscount"      BOOLEAN DEFAULT FALSE NOT NULL,
    "PercentDiscount" INTEGER               NOT NULL,
    "RuleId"          INTEGER               NOT NULL,
    "ProductType"     VARCHAR(100)          NOT NULL,
    "ProductPrice"    NUMERIC(12, 2)
);