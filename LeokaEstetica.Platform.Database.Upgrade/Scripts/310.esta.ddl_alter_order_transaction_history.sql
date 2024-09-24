ALTER TABLE commerce.order_transaction_history
    ADD COLUMN order_type commerce.ORDER_TYPE_ENUM NOT NULL;