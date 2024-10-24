CREATE OR REPLACE FUNCTION commerce.order_transactions_history_insert()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO commerce.order_transaction_history (created_at, action_text, action_sys_name, 
													  created_by, order_id, order_type, status_name)
    VALUES (NEW.created_at, 'Создание статуса заказа.', 
			'Insert', NEW.created_by, NEW.order_id, NEW.order_type, NEW.status_name);
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER order_transactions_history_insert_trigger
	AFTER INSERT 
	ON commerce.orders
	FOR EACH ROW
	EXECUTE FUNCTION commerce.order_transactions_history_insert()