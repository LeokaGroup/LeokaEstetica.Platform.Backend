CREATE OR REPLACE FUNCTION commerce.order_transactions_history_insert()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO commerce.order_transaction_history (created_at, action_text, action_sys_name, 
													  created_by, order_id, order_type)
    VALUES (NEW.created_at, 'Создание статуса заказа.', 
			'Insert', NEW.created_by, NEW.order_id, NEW.order_type);
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER order_transactions_history_insert_trigger
	AFTER INSERT 
	ON commerce.orders
	FOR EACH ROW
	EXECUTE FUNCTION commerce.order_transactions_history_insert()