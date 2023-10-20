CREATE FUNCTION "Commerce"."OrderTransactionsUpdate"() RETURNS TRIGGER
    LANGUAGE plpgsql
AS
$$
BEGIN
    NEW."StatusName" = NEW."StatusName",
                       NEW."StatusSysName" = NEW."StatusSysName";
END;
$$;

CREATE TRIGGER "OrderTransactionsShadowUpdateTrigger"
    AFTER INSERT
    ON "Commerce"."Orders"
    FOR EACH ROW
EXECUTE PROCEDURE "OrderTransactionsUpdate"();