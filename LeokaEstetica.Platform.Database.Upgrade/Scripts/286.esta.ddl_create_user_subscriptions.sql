CREATE TABLE subscriptions.user_subscriptions
(
    user_subscription_id BIGSERIAL NOT NULL,
    user_id              BIGINT    NOT NULL,
    is_active            BOOLEAN   NOT NULL DEFAULT FALSE,
    month_count          INT2      NOT NULL DEFAULT 1,
    subscription_id      BIGINT    NOT NULL,
    CONSTRAINT pk_user_subscriptions_user_subscription_id PRIMARY KEY (user_subscription_id),
    CONSTRAINT fk_all_subscriptions_subscription_id FOREIGN KEY (subscription_id) REFERENCES subscriptions.all_subscriptions (subscription_id)
);

COMMENT ON TABLE subscriptions.user_subscriptions IS 'Таблица подписок пользователя на тарифы.';
COMMENT ON COLUMN subscriptions.user_subscriptions.user_subscription_id IS 'PK.';
COMMENT ON COLUMN subscriptions.user_subscriptions.user_id IS 'Id пользователя.';
COMMENT ON COLUMN subscriptions.user_subscriptions.is_active IS 'Признак активности подписки.';
COMMENT ON COLUMN subscriptions.user_subscriptions.month_count IS 'Кол-во месяцев, на которое оформлена подписка пользователя..';
COMMENT ON COLUMN subscriptions.user_subscriptions.subscription_id IS 'Id подписки.';