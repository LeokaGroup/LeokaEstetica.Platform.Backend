INSERT INTO project_management.panel_items (item_name, item_url, position, panel_type, items, has_items, is_disabled,
                                            control_type, destination)
VALUES ('Левая панель модуля управления проектами', NULL, 0, 'ProjectManagementLeftPanel', '{
    "Items": [
        {
            "Disabled": false,
            "ItemName": "Пространства",
            "Position": 1,
            "Id": "Space",
            "IsFooterItem": false
        },
         {
            "Disabled": false,
            "ItemName": "Wiki",
            "Position": 2,
            "Id": "Wiki",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Задачи",
            "Position": 3,
            "Id": "Tasks",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Бэклог",
            "Position": 4,
            "Id": "Backlog",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Спринты",
            "Position": 5,
            "Id": "Sprints",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Дорожные карты",
            "Position": 6,
            "Id": "Roadmaps",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Отчеты",
            "Position": 7,
            "Id": "Reports",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Дашборды",
            "Position": 8,
            "Id": "Dashboards",
            "IsFooterItem": false
        },
        {
            "Disabled": false,
            "ItemName": "Трудозатраты",
            "Position": 9,
            "Id": "Time",
            "IsFooterItem": false
        },
         {
            "Disabled": false,
            "ItemName": "Уведомления",
            "Position": 10,
            "Id": "Notifications",
            "IsFooterItem": true
        },
         {
            "Disabled": false,
            "ItemName": "Релизы",
            "Position":11,
            "Id": "Releases",
            "IsFooterItem": false
        }
    ]
}', TRUE, FALSE, 'dropdown', 'LeftPanel');
