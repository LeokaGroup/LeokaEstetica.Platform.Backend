ALTER TABLE subscriptions.all_subscriptions
    ADD COLUMN rule_id INT NOT NULL;

ALTER TABLE subscriptions.all_subscriptions
    ADD CONSTRAINT fk_all_subscriptions_rule_id FOREIGN KEY (rule_id) REFERENCES rules.fare_rules (rule_id);

CREATE UNIQUE INDEX all_subscriptions_object_id_rule_id_subscription_type ON subscriptions.all_subscriptions (object_id, rule_id, subscription_type);