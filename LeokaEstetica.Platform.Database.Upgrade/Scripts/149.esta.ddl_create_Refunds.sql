CREATE TABLE "Commerce"."Refunds"
(
    "RefundId"      BIGSERIAL
        CONSTRAINT "PK_Refunds_RefundId"
            PRIMARY KEY,
    "PaymentId"     VARCHAR(50),
    "Price"         NUMERIC(12, 2)          NOT NULL,
    "DateCreated"   TIMESTAMP DEFAULT NOW() NOT NULL,
    "Status"        VARCHAR(50)             NOT NULL,
    "RefundOrderId" VARCHAR(50)             NOT NULL,
    "IsManual"      BOOLEAN   DEFAULT TRUE  NOT NULL
);