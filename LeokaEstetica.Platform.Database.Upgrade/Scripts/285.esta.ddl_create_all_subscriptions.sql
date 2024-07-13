CREATE TABLE subscriptions.all_subscriptions
(
    subscription_id   SERIAL                               NOT NULL,
    object_id         INT                                  NOT NULL,
    subscription_type subscriptions.SUBSCRIPTION_TYPE_ENUM NOT NULL,
    CONSTRAINT pk_all_subscriptions_subscription_id PRIMARY KEY (subscription_id)
);

COMMENT ON TABLE subscriptions.all_subscriptions IS 'Таблица подписок на тарифы.';
COMMENT ON COLUMN subscriptions.all_subscriptions.subscription_id IS 'PK.';
COMMENT ON COLUMN subscriptions.all_subscriptions.object_id IS 'Id объекта.';
COMMENT ON COLUMN subscriptions.all_subscriptions.subscription_type IS 'Тип подписки.';