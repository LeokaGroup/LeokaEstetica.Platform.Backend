CREATE TABLE commerce.order_transaction_history
(
    shadow_id       BIGSERIAL                NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    action_text     VARCHAR(150)             NOT NULL,
    action_sys_name VARCHAR(50)              NOT NULL,
    created_by      BIGINT                   NOT NULL,
    order_id        BIGINT                   NOT NULL,
    CONSTRAINT pk_shadow_id PRIMARY KEY (shadow_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId"),
    CONSTRAINT fk_orders_order_id FOREIGN KEY (order_id) REFERENCES commerce.orders (order_id)
);

COMMENT ON TABLE commerce.order_transaction_history IS 'Таблица заказов.';
COMMENT ON COLUMN commerce.order_transaction_history.shadow_id IS 'PK.';
COMMENT ON COLUMN commerce.order_transaction_history.created_at IS 'Дата создания записи.';
COMMENT ON COLUMN commerce.order_transaction_history.action_text IS 'Описание события.';
COMMENT ON COLUMN commerce.order_transaction_history.action_sys_name IS 'Системное описание события.';
COMMENT ON COLUMN commerce.order_transaction_history.created_by IS 'Id пользователя, который совершил транзакцию.';
COMMENT ON COLUMN commerce.order_transaction_history.order_id IS 'Id заказа.';