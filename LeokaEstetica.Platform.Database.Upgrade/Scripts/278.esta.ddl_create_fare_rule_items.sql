CREATE TABLE rules.fare_rule_items
(
    rule_item_id SERIAL  NOT NULL,
    rule_id      INT     NOT NULL,
    item_text    TEXT    NOT NULL,
    is_free      BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_rule_item_id PRIMARY KEY (rule_item_id),
    CONSTRAINT fk_fare_rules_rule_id FOREIGN KEY (rule_id) REFERENCES rules.fare_rules (rule_id)
);

COMMENT ON TABLE rules.fare_rule_items IS 'Таблица элементов тарифов.';
COMMENT ON COLUMN rules.fare_rule_items.rule_item_id IS 'PK.';
COMMENT ON COLUMN rules.fare_rule_items.rule_id IS 'Id тарифа.';
COMMENT ON COLUMN rules.fare_rule_items.item_text IS 'Описание тарифа (его элементы).';
COMMENT ON COLUMN rules.fare_rule_items.is_free IS 'Признак бесплатного тарифа.';