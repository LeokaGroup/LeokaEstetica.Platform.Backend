CREATE TABLE rules.fare_rule_attributes
(
    attribute_id      SERIAL       NOT NULL,
    attribute_name    VARCHAR(150) NOT NULL,
    attribute_details TEXT         NOT NULL,
    is_price          BOOLEAN      NOT NULL DEFAULT FALSE,
    rule_id           INT          NOT NULL,
    price NUMERIC (12, 2) NULL,
    measure VARCHAR(100) NOT NULL,
    CONSTRAINT pk_fare_rule_attributes_attribute_id PRIMARY KEY (attribute_id),
    CONSTRAINT fk_fare_rules_rule_id FOREIGN KEY (rule_id) REFERENCES rules.fare_rules (rule_id)
);

COMMENT ON TABLE rules.fare_rule_attributes IS 'Таблица элементов тарифов.';
COMMENT ON COLUMN rules.fare_rule_attributes.attribute_id IS 'PK.';
COMMENT ON COLUMN rules.fare_rule_attributes.attribute_name IS 'Название атрибута тарифа';
COMMENT ON COLUMN rules.fare_rule_attributes.attribute_details IS 'Описание атрибутов тарифа.';
COMMENT ON COLUMN rules.fare_rule_attributes.is_price IS 'Наличие цены у тарифа.';
COMMENT ON COLUMN rules.fare_rule_attributes.rule_id IS 'Id тарифа.';
COMMENT ON COLUMN rules.fare_rule_attributes.price IS 'Цена тарифа.';
COMMENT ON COLUMN rules.fare_rule_attributes.measure IS 'Ед.изм. тарифа.';