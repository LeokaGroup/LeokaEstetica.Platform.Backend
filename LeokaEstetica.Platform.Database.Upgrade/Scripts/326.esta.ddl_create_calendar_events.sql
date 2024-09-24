CREATE TABLE project_management_human_resources.calendar_events
(
    event_id          BIGINT       NOT NULL,
    event_name        VARCHAR(150) NOT NULL,
    event_description TEXT         NULL,
    created_by        BIGINT       NOT NULL,
    created_at        TIMESTAMP    NOT NULL DEFAULT NOW(),
    event_start_date  TIMESTAMP    NOT NULL,
    event_end_date    TIMESTAMP    NOT NULL,
    event_location    VARCHAR(200) NULL,
    CONSTRAINT pk_calendar_events_event_id PRIMARY KEY (event_id),
    CONSTRAINT fk_users_user_id FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE project_management_human_resources.calendar_events IS 'Таблица событий календаря.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.event_id IS 'PK.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.event_name IS 'Название события.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.event_description IS 'Описание события.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.created_by IS 'Id пользователя, который создал событие.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.created_at IS 'Дата создания события.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.event_start_date IS 'Дата начала события.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.event_end_date IS 'Дата окончания события.';
COMMENT ON COLUMN project_management_human_resources.calendar_events.event_location IS 'Место проведения события (адрес или место).';