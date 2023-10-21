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