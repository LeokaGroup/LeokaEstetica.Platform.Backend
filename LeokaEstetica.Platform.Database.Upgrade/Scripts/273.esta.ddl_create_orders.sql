CREATE TABLE commerce.orders
(
    order_id        BIGSERIAL                NOT NULL,
    order_name      VARCHAR(200)             NOT NULL,
    order_details   VARCHAR(300)             NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by      BIGINT                   NOT NULL,
    status_name     VARCHAR(50)              NOT NULL DEFAULT 'Новый',
    status_sys_name VARCHAR(50)              NOT NULL DEFAULT 'New',
    price           DECIMAL(12, 2)           NOT NULL,
    vat_rate        INT                      NULL,
    price_vat       DECIMAL(12, 2)           NULL,
    discount        INT                      NULL,
    discount_price  DECIMAL(12, 2)           NULL,
    total_price     DECIMAL(12, 2)           NOT NULL,
    currency        commerce.CURRENCY_ENUM   NOT NULL,
    payment_month   INT                      NOT NULL,
    payment_id      VARCHAR(50)              NOT NULL,
    CONSTRAINT pk_order_id PRIMARY KEY (order_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE commerce.orders IS 'Таблица заказов.';
COMMENT ON COLUMN commerce.orders.order_id IS 'PK.';
COMMENT ON COLUMN commerce.orders.order_name IS 'Название заказа.';
COMMENT ON COLUMN commerce.orders.order_details IS 'Описание заказа.';
COMMENT ON COLUMN commerce.orders.created_at IS 'Дата создания заказа.';
COMMENT ON COLUMN commerce.orders.created_by IS 'Id пользователя, который оформил заказ.';
COMMENT ON COLUMN commerce.orders.status_name IS 'Название статуса.';
COMMENT ON COLUMN commerce.orders.status_sys_name IS 'Системное название статуса.';
COMMENT ON COLUMN commerce.orders.price IS 'Цена заказа.';
COMMENT ON COLUMN commerce.orders.vat_rate IS 'Ставка НДС в %.';
COMMENT ON COLUMN commerce.orders.price_vat IS 'Цена с НДС (цена без НДС + НДС).';
COMMENT ON COLUMN commerce.orders.discount IS '% скидки.';
COMMENT ON COLUMN commerce.orders.discount_price IS 'Цена со скидкой.';
COMMENT ON COLUMN commerce.orders.total_price IS 'Общая сумма заказа (вместе со скидками и НДС - если они были).';
COMMENT ON COLUMN commerce.orders.payment_month IS 'Кол-во месяцев, на которое оформляется заказ.';
COMMENT ON COLUMN commerce.orders.payment_id IS 'Id платежа в ПС.';