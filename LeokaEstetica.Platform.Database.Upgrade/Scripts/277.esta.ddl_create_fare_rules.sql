CREATE TABLE rules.fare_rules
(
    rule_id             SERIAL         NOT NULL,
    rule_name           VARCHAR(150)   NOT NULL,
    employees_count     INT            NOT NULL DEFAULT 1,
    feature             VARCHAR(150)   NOT NULL,
    price               NUMERIC(12, 2) NULL,
    post_vacancy        VARCHAR(100)   NOT NULL,
    access_user_profile VARCHAR(100)   NOT NULL,
    CONSTRAINT pk_rule_id PRIMARY KEY (rule_id)
);

COMMENT ON TABLE rules.fare_rules IS 'Таблица тарифов.';
COMMENT ON COLUMN rules.fare_rules.rule_id IS 'PK.';
COMMENT ON COLUMN rules.fare_rules.rule_name IS 'Название тарифа.';
COMMENT ON COLUMN rules.fare_rules.employees_count IS 'Кол-во сотрудников.';
COMMENT ON COLUMN rules.fare_rules.feature IS 'Функционал.';
COMMENT ON COLUMN rules.fare_rules.price IS 'Цена тарифа.';
COMMENT ON COLUMN rules.fare_rules.post_vacancy IS 'Размещение  вакансий.';
COMMENT ON COLUMN rules.fare_rules.access_user_profile IS 'Доступ к базе резюме.';