INSERT INTO settings.move_not_completed_tasks_settings (name, sys_name, tooltip, selected, disabled, project_id, user_id)
VALUES ('Бэклог проекта', 'Backlog', '(выбрано по умолчанию)', TRUE, FALSE, 4, 2),
       ('В следующий спринт (если у проекта имеется минимум 1 запланированный спринт следующий за активным)', 'NextSprint', NULL, FALSE, FALSE, 4, 2);