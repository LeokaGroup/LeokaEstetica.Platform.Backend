CREATE OR REPLACE FUNCTION commerce.order_transactions_history_update()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO commerce.order_transaction_history (created_at, action_text, action_sys_name, 
													  created_by, order_id, order_type, status_name)
    VALUES (OLD.created_at, 'Изменение статуса заказа.', 
			'Update', OLD.created_by, OLD.order_id, OLD.order_type, NEW.status_name);
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER order_transactions_history_update_trigger
	AFTER UPDATE 
	ON commerce.orders
	FOR EACH ROW
	EXECUTE FUNCTION commerce.order_transactions_history_update()