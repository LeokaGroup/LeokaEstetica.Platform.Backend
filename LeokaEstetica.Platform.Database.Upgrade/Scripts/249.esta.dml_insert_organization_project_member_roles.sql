INSERT INTO roles.organization_project_member_roles (organization_id, organization_member_id, role_name,
                                                     role_sys_name, is_active, is_enabled, project_id)
VALUES (3, 32, 'Настройки проекта', 'ProjectSettings', TRUE, TRUE, 274),
       (3, 32, 'Настройки представления', 'ProjectStrategySettings', TRUE, TRUE, 274),
       (3, 32, 'Создание задачи (любого типа)', 'ProjectCreateTaskSettings', TRUE, TRUE, 274),
       (3, 32, 'Отчеты', 'ProjectReportsSettings', TRUE, TRUE, 274),
       (3, 32, 'Дашборды', 'ProjectDashboardSettings', TRUE, TRUE, 274),
       (3, 32, 'Дорожные карты проекта', 'ProjectMapSettings', TRUE, TRUE, 274),
       (3, 32, 'Релизы', 'ProjectReleaseSettings', TRUE, TRUE, 274),
       (3, 32, 'Wiki', 'ProjectWikiSettings', TRUE, TRUE, 274),
       (3, 32, 'Доступ к проекту', 'Project', TRUE, TRUE, 274),
       (3, 32, 'Доступ к компании', 'ProjectWiki', TRUE, TRUE, 274),
       (3, 32, 'Создание спринта', 'ProjectCreateSprintSettings', TRUE, TRUE, 274),
       (3, 32, 'Доступ к спринту', 'Sprint', TRUE, TRUE, 274);