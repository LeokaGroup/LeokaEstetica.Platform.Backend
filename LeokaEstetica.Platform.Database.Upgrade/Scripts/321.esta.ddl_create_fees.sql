CREATE TABLE commerce.fees
(
    fees_id           SERIAL         NOT NULL,
    fees_name         VARCHAR(150)   NOT NULL,
    fees_sys_name     VARCHAR(70)    NOT NULL,
    fees_price        NUMERIC(12, 2) NULL,
    fees_measure      VARCHAR(70)    NULL,
    fees_fare_rule_id INT            NOT NULL,
    fees_is_active    BOOLEAN        NOT NULL DEFAULT TRUE,
    CONSTRAINT pk_fees_fees_id PRIMARY KEY (fees_id),
    CONSTRAINT fk_fare_rule_rule_Id FOREIGN KEY (fees_fare_rule_id) REFERENCES rules.fare_rules (rule_id)
);

COMMENT ON TABLE commerce.fees IS 'Таблица услуг.';
COMMENT ON COLUMN commerce.fees.fees_id IS 'PK.';
COMMENT ON COLUMN commerce.fees.fees_name IS 'Название услуги.';
COMMENT ON COLUMN commerce.fees.fees_sys_name IS 'Системное название услуги.';
COMMENT ON COLUMN commerce.fees.fees_price IS 'Цена услуги. Может быть NULL (тогда бесплатно).';
COMMENT ON COLUMN commerce.fees.fees_measure IS 'Ед.изм.';
COMMENT ON COLUMN commerce.fees.fees_fare_rule_id IS 'Id тарифа. Может быть NULL (тогда услуга не связана с тарифом).';
COMMENT ON COLUMN commerce.fees.fees_is_active IS 'Признак активной услуги.';