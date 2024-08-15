ALTER TABLE commerce.orders
    ALTER COLUMN payment_month
        DROP NOT NULL;

ALTER TABLE commerce.orders
    ADD COLUMN order_type commerce.ORDER_TYPE_ENUM NOT NULL;