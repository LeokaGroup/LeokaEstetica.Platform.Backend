CREATE OR REPLACE FUNCTION order_transactions_update_tr() RETURNS TRIGGER
    LANGUAGE plpgsql
AS
$$
BEGIN
INSERT INTO commerce.order_transaction_history (created_at, action_text, action_sys_name, created_by, order_id, order_type)
VALUES (OLD.created_at, 'Изменение статуса заказа.', 'Update', OLD.created_by, OLD.order_id, OLD.order_type);

RETURN NEW;
END ;
$$;