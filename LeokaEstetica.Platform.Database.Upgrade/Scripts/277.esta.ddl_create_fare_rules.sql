CREATE TABLE rules.fare_rules
(
    rule_id             SERIAL         NOT NULL,
    rule_name           VARCHAR(150)   NOT NULL,
    position INT NOT NULL DEFAULT 0,
    is_free      BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_rule_id PRIMARY KEY (rule_id)
);

COMMENT ON TABLE rules.fare_rules IS 'Таблица тарифов.';
COMMENT ON COLUMN rules.fare_rules.rule_id IS 'PK.';
COMMENT ON COLUMN rules.fare_rules.rule_name IS 'Название тарифа.';
COMMENT ON COLUMN rules.fare_rules.position IS 'Позиция.';
COMMENT ON COLUMN rules.fare_rules.is_free IS 'Признак бесплатного тарифа.';