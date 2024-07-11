CREATE FUNCTION order_transaction_history() RETURNS trigger
    LANGUAGE plpgsql
AS
$$
BEGIN
INSERT INTO commerce.order_transaction_history (created_at, action_text, action_sys_name, created_by, order_id)
VALUES (OLD.created_at, 'Изменение статуса заказа.', 'Update', OLD.created_by, OLD.order_id);

RETURN NEW;
END;
$$;