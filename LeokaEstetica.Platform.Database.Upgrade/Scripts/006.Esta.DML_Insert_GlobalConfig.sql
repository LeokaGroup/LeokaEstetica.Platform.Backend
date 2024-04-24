create schema "Configs";

CREATE TABLE "Configs"."GlobalConfig"
(
    "ParamId"          BIGSERIAL,
    "ParamKey"         VARCHAR(200) NOT NULL,
    "ParamValue"       TEXT         NOT NULL,
    "ParamType"        VARCHAR(50)  NOT NULL,
    "ParamDescription" VARCHAR(200) NOT NULL,
    "ParamTag"         VARCHAR(50)
);

INSERT INTO "Configs"."GlobalConfig" ("ParamKey", "ParamValue", "ParamType", "ParamDescription", "ParamTag")
VALUES ('Commerce.Test.Price.Mode.Enabled', 'False', 'Boolean',
        'Ключ вкл/откл режим тестовой цены для тестирования оплаты в ПС на реальной цене заказа.',
        'Payment system test mode'),
       ('Commerce.Test.Price.Enabled.Value', '10', 'Decimal',
        'Ключ тестовой цены для тестирования оплаты в ПС на реальной цене заказа.', 'Payment system test mode');