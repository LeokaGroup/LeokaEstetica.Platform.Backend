INSERT INTO dbo.left_menu (items)
VALUES ('{
  "items": [
    {
      "label": "Профиль",
      "icon": null,
      "items": [
        {
          "label": "Просмотр анкеты",
          "icon": null,
          "items": null,
          "url": "/profile/aboutme",
          "disabled": false,
          "visible": true,
          "id": "ViewProfile",
          "tooltip": null,
          "tooltipPosition": null,
          "position": null,
          "command": null,
          "route": "/profile/aboutme",
          "queryParams": {
            "mode": "view"
          }
        },
        {
          "label": "Изменить анкету",
          "icon": null,
          "items": null,
          "url": "/profile/aboutme",
          "disabled": false,
          "visible": true,
          "id": "EditProfile",
          "tooltip": null,
          "tooltipPosition": null,
          "position": null,
          "command": null,
          "route": "/profile/aboutme",
          "queryParams": {
            "mode": "edit"
          }
        }
      ],
      "url": null,
      "disabled": false,
      "visible": true,
      "id": "Profile",
      "tooltip": null,
      "tooltipPosition": null,
      "position": 1,
      "command": null,
      "route": null
    },
    {
      "separator": true
    },
    {
      "label": "Модули",
      "icon": null,
      "items": [
        {
          "label": "Управление проектами",
          "icon": null,
          "items": [
            {
              "label": "Пространства",
              "icon": null,
              "items": [],
              "url": "/project-management/workspaces",
              "disabled": false,
              "visible": true,
              "id": "WorkSpaces",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 1,
              "command": null,
              "route": "/project-management/workspaces"
            },
            {
              "label": "Wiki",
              "icon": null,
              "items": [],
              "url": "/project-management/wiki",
              "disabled": false,
              "visible": true,
              "id": "Wiki",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 2,
              "command": null,
              "route": "/project-management/wiki"
            },
            {
              "label": "Задачи",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": true,
              "id": "Tasks",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 3,
              "command": null,
              "route": null
            },
            {
              "label": "Бэклог",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": true,
              "id": "Backlog",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 4,
              "command": null,
              "route": null
            },
            {
              "label": "Спринты",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": true,
              "id": "Sprints",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 5,
              "command": null,
              "route": null
            },
            {
              "label": "Дорожные карты",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": false,
              "id": "Roadmaps",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 6,
              "command": null,
              "route": null
            },
            {
              "label": "Отчеты",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": false,
              "id": "Reports",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 7,
              "command": null,
              "route": null
            },
            {
              "label": "Дашборды",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": false,
              "id": "Dashboards",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 8,
              "command": null,
              "route": null
            },
            {
              "label": "Трудозатраты",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": false,
              "id": "Timesheets",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 9,
              "command": null,
              "route": null
            },
            {
              "label": "Релизы",
              "icon": null,
              "items": [],
              "url": null,
              "disabled": false,
              "visible": false,
              "id": "Releases",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 10,
              "command": null,
              "route": null
            }
          ],
          "url": null,
          "disabled": false,
          "visible": true,
          "id": "ProjectManagement",
          "tooltip": null,
          "tooltipPosition": null,
          "position": 1,
          "command": null,
          "route": null
        },
        {
          "label": "Управление персоналом (HR)",
          "icon": null,
          "items": [
            {
              "label": "Создать вакансию",
              "icon": null,
              "items": null,
              "url": "/vacancies/create",
              "disabled": false,
              "visible": true,
              "id": "CreateVacancy",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 1,
              "command": null,
              "route": "/vacancies/create"
            },
            {
              "label": "Ваши вакансии",
              "icon": null,
              "items": null,
              "url": "/vacancies/my",
              "disabled": false,
              "visible": true,
              "id": "UserVacancies",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 2,
              "command": null,
              "route": "/vacancies/my"
            },
            {
              "label": "Вакансии в архиве",
              "icon": null,
              "items": null,
              "url": "/vacancies/archive",
              "disabled": false,
              "visible": true,
              "id": "ArchivedVacancies",
              "tooltip": null,
              "tooltipPosition": null,
              "position": 3,
              "command": null,
              "route": "/vacancies/archive"
            }
          ],
          "url": null,
          "disabled": false,
          "visible": true,
          "id": "HR",
          "tooltip": null,
          "tooltipPosition": null,
          "position": 2,
          "command": null,
          "route": null
        }
      ],
      "url": null,
      "disabled": false,
      "visible": true,
      "id": "Profile",
      "tooltip": null,
      "tooltipPosition": null,
      "position": 2,
      "command": null,
      "route": null
    }
  ]
}');