TRUNCATE TABLE project_management.header;

INSERT INTO project_management.header (header_id, item_name, item_url, position, header_type, items, has_items,
                                       is_disabled)
VALUES (9, 'Стратегия представления', NULL, 1, 'ProjectManagment', '{
  "Items": [
    {
      "Id": "Kanban",
      "Items": null,
      "ItemUrl": null,
      "Disabled": false,
      "HasItems": false,
      "ItemName": "Kanban (доска)",
      "Position": 1
    },
    {
      "Id": "Scrum",
      "Items": null,
      "ItemUrl": null,
      "Disabled": false,
      "HasItems": false,
      "ItemName": "Scrum (список)",
      "Position": 2
    }
  ]
}', TRUE, TRUE),
       (10, 'Создать', NULL, 2, 'ProjectManagment', '{
         "Items": [
           {
             "Id": "Task",
             "Disabled": true,
             "ItemName": "Задачу",
             "Position": 1
           }
         ]
       }', TRUE, FALSE),
       (11, 'Фильтры', NULL, 3, 'ProjectManagment', '{
         "Items": [
           {
             "Id": "Name",
             "Disabled": false,
             "ItemName": "Название",
             "Position": 1
           },
           {
             "Id": "Executor",
             "Disabled": false,
             "ItemName": "Исполнитель",
             "Position": 2
           },
           {
             "Id": "Priority",
             "Disabled": false,
             "ItemName": "Приоритет",
             "Position": 3
           },
           {
             "Id": "Tag",
             "Disabled": false,
             "ItemName": "Метка",
             "Position": 4
           },
           {
             "Id": "Parent",
             "Disabled": false,
             "ItemName": "Родительская карточка",
             "Position": 5
           },
           {
             "Id": "Member",
             "Disabled": false,
             "ItemName": "Участник",
             "Position": 6
           },
           {
             "Id": "Child",
             "Disabled": false,
             "ItemName": "Дочерняя карточка",
             "Position": 7
           },
           {
             "Id": "Status",
             "Disabled": false,
             "ItemName": "Статус",
             "Position": 8
           },
           {
             "Id": "Type",
             "Disabled": false,
             "ItemName": "Тип задачи",
             "Position": 9
           },
           {
             "Disabled": false,
             "ItemName": "Дата завершения",
             "Position": 10
           },
           {
             "Id": "Created",
             "Disabled": false,
             "ItemName": "Дата создания",
             "Position": 11
           },
           {
             "Id": "LastMoved",
             "Disabled": false,
             "ItemName": "Последнее перемещение",
             "Position": 12
           },
           {
             "Id": "Updated",
             "Disabled": false,
             "ItemName": "Дата обновления",
             "Position": 13
           },
           {
             "Id": "CreatedStartWork",
             "Disabled": false,
             "ItemName": "Дата взятия в работу",
             "Position": 14
           },
           {
             "Id": "CreatedStart",
             "Disabled": false,
             "ItemName": "Дата планируемого начала",
             "Position": 15
           },
           {
             "Id": "CreatedEnd",
             "Disabled": false,
             "ItemName": "Дата планируемого окончания",
             "Position": 16
           },
           {
             "Id": "Deadline",
             "Disabled": false,
             "ItemName": "Срок",
             "Position": 17
           },
           {
             "Id": "DeployDanger",
             "Disabled": false,
             "ItemName": "Деплой опасность",
             "Position": 18
           }
         ]
       }', TRUE, FALSE),
       (12, 'Настройки', NULL, 4, 'ProjectManagment', '{
         "Items": [
           {
             "Id": "ViewSettings",
             "Items": null,
             "ItemUrl": null,
             "Disabled": false,
             "HasItems": false,
             "ItemName": "Настройки представлений",
             "Position": 1
           },
           {
             "Id": "ProjectSettings",
             "Items": null,
             "ItemUrl": null,
             "Disabled": false,
             "HasItems": false,
             "ItemName": "Настройки проекта",
             "Position": 2
           }
         ]
       }', TRUE, FALSE);