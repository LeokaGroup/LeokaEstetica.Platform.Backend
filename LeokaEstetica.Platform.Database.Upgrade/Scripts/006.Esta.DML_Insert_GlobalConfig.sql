INSERT INTO "Configs"."GlobalConfig" ("ParamKey", "ParamValue", "ParamType", "ParamDescription", "ParamTag")
VALUES ('Commerce.Test.Price.Mode.Enabled', 'False', 'Boolean',
        'Ключ вкл/откл режим тестовой цены для тестирования оплаты в ПС на реальной цене заказа.',
        'Payment system test mode'),
       ('Commerce.Test.Price.Enabled.Value', '10', 'Decimal',
        'Ключ тестовой цены для тестирования оплаты в ПС на реальной цене заказа.', 'Payment system test mode');