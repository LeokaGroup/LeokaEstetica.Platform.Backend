CREATE TABLE "Commerce"."Receipts"
(
    "ReceiptId"      BIGSERIAL
        CONSTRAINT "PK_Receipts_ReceiptId"
            PRIMARY KEY,
    "OrderId"        BIGINT                  NOT NULL,
    "DateCreated"    TIMESTAMP DEFAULT NOW() NOT NULL,
    "ReceiptOrderId" VARCHAR(50)             NOT NULL,
    "PaymentId"      VARCHAR(50)             NOT NULL,
    "Status"         VARCHAR(50)             NOT NULL,
    "Type"           VARCHAR(50)             NOT NULL,
    "UserId"         BIGINT                  NOT NULL
);