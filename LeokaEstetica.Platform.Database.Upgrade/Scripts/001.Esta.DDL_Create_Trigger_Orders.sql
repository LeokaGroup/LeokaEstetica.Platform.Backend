create schema "Commerce";

CREATE TABLE "Commerce"."Orders"
(
    "OrderId"       BIGSERIAL
        CONSTRAINT "PK_Orders_OrderId"
            PRIMARY KEY,
    "OrderName"     VARCHAR(200)                                      NOT NULL,
    "OrderDetails"  VARCHAR(300)                                      NOT NULL,
    "DateCreated"   TIMESTAMP      DEFAULT NOW()                      NOT NULL,
    "UserId"        BIGINT                                            NOT NULL,
    "StatusName"    VARCHAR(50)    DEFAULT 'Новый'::CHARACTER VARYING NOT NULL,
    "StatusSysName" VARCHAR(50)    DEFAULT 'New'::CHARACTER VARYING   NOT NULL,
    "Price"         NUMERIC(12, 2) DEFAULT 0                          NOT NULL,
    "Currency"      VARCHAR(5)     DEFAULT 'RUB'::CHARACTER VARYING   NOT NULL,
    "PaymentMonth"  SMALLINT       DEFAULT 1                          NOT NULL,
    "PaymentId"     VARCHAR(50)                                       NOT NULL
);

CREATE OR REPLACE FUNCTION "Commerce"."OrderTransactionsUpdate"() RETURNS TRIGGER
    LANGUAGE plpgsql
AS
$$
BEGIN
    INSERT INTO "Commerce"."OrderTransactionsShadow" ("DateCreated", "ActionText", "ActionSysName", "UserId", "OrderId",
                                                      "StatusName")
    VALUES (OLD."DateCreated", 'Изменение статуса заказа.', 'Update', OLD."UserId", OLD."OrderId", NEW."StatusName");

    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS "OrderTransactionsShadowUpdateTrigger" ON "Commerce"."Orders";

CREATE TRIGGER "OrderTransactionsShadowUpdateTrigger"
    AFTER UPDATE
    ON "Commerce"."Orders"
    FOR EACH ROW
EXECUTE PROCEDURE "Commerce"."OrderTransactionsUpdate"();