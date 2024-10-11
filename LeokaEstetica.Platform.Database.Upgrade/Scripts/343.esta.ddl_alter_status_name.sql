ALTER TABLE commerce.order_transaction_history
	ADD COLUMN IF NOT EXISTS status_name character varying(50) NOT NULL;