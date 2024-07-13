CREATE TABLE rules.fare_rule_attributes
(
    attribute_id      SERIAL       NOT NULL,
    attribute_name    VARCHAR(150) NOT NULL,
    attribute_details TEXT         NOT NULL,
    position          INT          NOT NULL DEFAULT 0,
    CONSTRAINT pk_fare_rule_attributes_attribute_id PRIMARY KEY (attribute_id)
);

COMMENT ON TABLE rules.fare_rule_attributes IS 'Таблица элементов тарифов.';
COMMENT ON COLUMN rules.fare_rule_attributes.attribute_id IS 'PK.';
COMMENT ON COLUMN rules.fare_rule_attributes.attribute_name IS 'Название атрибута тарифа';
COMMENT ON COLUMN rules.fare_rule_attributes.attribute_details IS 'Описание атрибутов тарифа.';