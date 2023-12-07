INSERT INTO "ProjectManagment"."Header" ("ItemName", "ItemUrl", "Position", "HeaderType", "Items", "HasItems",
                                         "IsDisabled")
VALUES ('Стратегия представления', NULL, 1, 'ProjectManagment', '{
  "Items": [
    {
      "ItemName": "Kanban (доска)",
      "Position": 1,
      "ItemUrl": null,
      "IsDisabled": false,
      "HeaderItemId": 1,
      "HasItems": false,
      "Items": null
    },
    {
      "ItemName": "Scrum (список)",
      "Position": 2,
      "ItemUrl": null,
      "IsDisabled": false,
      "HeaderItemId": 1,
      "HasItems": false,
      "Items": null
    }
  ]
}', TRUE, FALSE),
       ('Создать', NULL, 2, 'ProjectManagment', '{
         "Items": [
           {
             "ItemName": "Задачу",
             "Position": 1
           }
         ]
       }', TRUE, FALSE),
       ('Фильтры', NULL, 3, 'ProjectManagment', '{
         "Items": [
           {
             "ItemName": "Название",
             "Position": 1,
             "IsDisabled": false
           },
           {
             "ItemName": "Исполнитель",
             "Position": 2,
             "IsDisabled": false
           },
           {
             "ItemName": "Приоритет",
             "Position": 3,
             "IsDisabled": false
           },
           {
             "ItemName": "Метка",
             "Position": 4,
             "IsDisabled": false
           },
           {
             "ItemName": "Родительская карточка",
             "Position": 5,
             "IsDisabled": false
           },
           {
             "ItemName": "Участник",
             "Position": 6,
             "IsDisabled": false
           },
           {
             "ItemName": "Дочерняя карточка",
             "Position": 7,
             "IsDisabled": false
           },
           {
             "ItemName": "Дочерняя карточка",
             "Position": 8,
             "IsDisabled": false
           },
           {
             "ItemName": "Статус",
             "Position": 9,
             "IsDisabled": false
           },
           {
             "ItemName": "Тип задачи",
             "Position": 10,
             "IsDisabled": false
           },
           {
             "ItemName": "Дата завершения",
             "Position": 11,
             "IsDisabled": false
           },
           {
             "ItemName": "Дата создания",
             "Position": 12,
             "IsDisabled": false
           },
           {
             "ItemName": "Последнее перемещение",
             "Position": 13,
             "IsDisabled": false
           },
           {
             "ItemName": "Дата обновления",
             "Position": 14,
             "IsDisabled": false
           },
           {
             "ItemName": "Дата взятия в работу",
             "Position": 15,
             "IsDisabled": false
           },
           {
             "ItemName": "Дата планируемого начала",
             "Position": 16,
             "IsDisabled": false
           },
           {
             "ItemName": "Дата планируемого окончания",
             "Position": 17,
             "IsDisabled": false
           },
           {
             "ItemName": "Срок",
             "Position": 18,
             "IsDisabled": false
           },
           {
             "ItemName": "Деплой опасность",
             "Position": 19,
             "IsDisabled": false
           }
         ]
       }', TRUE, FALSE),
       ('Настройки', NULL, 4, 'ProjectManagment', '[]', FALSE, FALSE);