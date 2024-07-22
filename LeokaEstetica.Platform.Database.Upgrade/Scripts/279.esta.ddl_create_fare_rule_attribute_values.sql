CREATE TABLE rules.fare_rule_attribute_values
(
    value_id        SERIAL  NOT NULL,
    rule_id         INT     NOT NULL,
    is_price        BOOLEAN NOT NULL DEFAULT FALSE,
    attribute_id    INT     NOT NULL,
    position        INT     NOT NULL DEFAULT 0,
    measure         VARCHAR(100) NULL,
    min_value       NUMERIC(12, 2) NULL,
    max_value       NUMERIC(12, 2) NULL,
    is_range        BOOLEAN NOT NULL DEFAULT FALSE,
    content_tooltip VARCHAR(150) NULL,
    content         TEXT    NOT NULL,
    CONSTRAINT pk_fare_rule_attribute_values_value_id PRIMARY KEY (value_id),
    CONSTRAINT fk_fare_rules_rule_id FOREIGN KEY (rule_id) REFERENCES rules.fare_rules (rule_id),
    CONSTRAINT fk_fare_rule_attributes_attribute_id FOREIGN KEY (attribute_id) REFERENCES rules.fare_rule_attributes (attribute_id)
);

COMMENT
ON TABLE rules.fare_rule_attribute_values IS 'Таблица элементов тарифов.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.value_id IS 'PK.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.rule_id IS 'Id тарифа.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.is_price IS 'Признак наличия цены.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.attribute_id IS 'Id атрибута.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.position IS 'Позиция.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.measure IS 'Ед.изм.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.min_value IS 'Значение От.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.max_value IS 'Значение До.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.is_range IS 'Признак наличия диапазона.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.content_tooltip IS 'Пояснение к контекту атрибута.';
COMMENT
ON COLUMN rules.fare_rule_attribute_values.content IS 'Контект атрибута.';