UPDATE dbo.top_menu
SET items = '{
  "items": [
    {
      "id": "Calendar",
      "icon": "pi pi-calendar",
      "items": [],
      "label": null,
      "command": null,
      "tooltip": "Календарь",
      "visible": true,
      "disabled": false,
      "position": 1,
      "routerLink": "/calendar/employee",
      "tooltipPosition": "bottom"
    },
    {
      "id": "Chat",
      "icon": "pi pi-comments",
      "items": [],
      "label": null,
      "command": null,
      "tooltip": "Коммуникации",
      "visible": true,
      "disabled": false,
      "position": 2,
      "routerLink": null,
      "tooltipPosition": "bottom"
    },
    {
      "id": "Notifications",
      "icon": "pi pi-bell",
      "items": [],
      "label": null,
      "command": null,
      "tooltip": "Уведомления",
      "visible": true,
      "disabled": false,
      "position": 3,
      "routerLink": null,
      "tooltipPosition": "bottom"
    },
    {
      "id": "Profile",
      "icon": "pi pi-user",
      "items": [
        {
          "id": "Profile",
          "icon": null,
          "items": [],
          "label": "Профиль",
          "command": null,
          "tooltip": null,
          "visible": true,
          "disabled": false,
          "position": 1,
          "routerLink": "/profile/aboutme",
          "tooltipPosition": null
        },
        {
          "id": "Orders",
          "icon": null,
          "items": [],
          "label": "Заказы",
          "command": null,
          "tooltip": null,
          "visible": true,
          "disabled": false,
          "position": 2,
          "routerLink": "/profile/orders",
          "tooltipPosition": null
        },
        {
          "id": "Tickets",
          "icon": null,
          "items": [],
          "label": "Тикеты",
          "command": null,
          "tooltip": null,
          "visible": true,
          "disabled": false,
          "position": 3,
          "routerLink": "/profile/tickets",
          "tooltipPosition": null
        },
        {
          "id": "Exit",
          "icon": null,
          "items": [],
          "label": "Выйти",
          "command": null,
          "tooltip": null,
          "visible": true,
          "disabled": false,
          "position": 4,
          "routerLink": null,
          "tooltipPosition": null
        }
      ],
      "label": null,
      "command": null,
      "tooltip": null,
      "visible": true,
      "disabled": false,
      "position": 4,
      "routerLink": null,
      "tooltipPosition": null
    }
  ]
}'
WHERE menu_id = 1;
