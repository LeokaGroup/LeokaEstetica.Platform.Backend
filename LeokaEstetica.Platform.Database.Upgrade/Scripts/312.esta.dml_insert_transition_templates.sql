INSERT INTO templates.project_management_transition_templates (transition_id, transition_name, transition_sys_name,
                                                               "position",
                                                               from_status_id, to_status_id)
VALUES (45, 'Переход из статуса "Новый" в статус "В работе"', 'NewToWork', 10, 13, 2),
       (46, 'Переход из статуса "В работе" в статус "Завершен"', 'InWorkToCompleted', 11, 2, 14),
       (47, 'Переход из статуса "В работе" в статус "Закрыт"', 'InWorkToClosed', 11, 2, 16),
       (48, 'Переход из статуса "Завершен" в статус "В работе"', 'CompletedToWork', 12, 14, 2),
       (49, 'Переход из статуса "Закрыт" в статус "В работе"', 'CompletedToWork', 12, 16, 2),
       (50, 'Переход из статуса "Новая" в статус "В работе"', 'CompletedToWork', 13, 4, 2),
       (51, 'Переход из статуса "В работе" в статус "Завершена"', 'InWorkToCompleted', 14, 2, 15),
       (52, 'Переход из статуса "В работе" в статус "Закрыта"', 'InWorkToClosed', 15, 2, 17),
       (53, 'Переход из статуса "Закрыта" в статус "Новая"', 'ClosedToNew', 16, 17, 4),
       (54, 'Переход из статуса "Завершена" в статус "Новая"', 'CompletedToNew', 17, 15, 4);