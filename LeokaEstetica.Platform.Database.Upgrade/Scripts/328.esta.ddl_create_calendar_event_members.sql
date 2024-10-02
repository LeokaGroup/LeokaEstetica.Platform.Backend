CREATE TABLE IF NOT EXISTS project_management_human_resources.calendar_event_members
(
    id              BIGINT                                                         NOT NULL,
    event_member_id BIGINT                                                         NOT NULL,
    event_id        BIGINT                                                         NOT NULL,
    member_status   project_management_human_resources.CALENDAR_MEMBER_STATUS_ENUM NOT NULL,
    joined          TIMESTAMP                                                      NOT NULL,
    CONSTRAINT pk_calendar_event_members_id PRIMARY KEY (id),
    CONSTRAINT fk_calendar_events_event_id FOREIGN KEY (event_id) REFERENCES project_management_human_resources.calendar_events (event_id)
);

COMMENT ON TABLE project_management_human_resources.calendar_event_members IS 'Таблица участников события календаря.';
COMMENT ON COLUMN project_management_human_resources.calendar_event_members.id IS 'PK.';
COMMENT ON COLUMN project_management_human_resources.calendar_event_members.event_member_id IS 'Id участника события.';
COMMENT ON COLUMN project_management_human_resources.calendar_event_members.event_id IS 'Id события.';
COMMENT ON COLUMN project_management_human_resources.calendar_event_members.member_status IS 'Статус участника события. Берется из енамки статусов.';
COMMENT ON COLUMN project_management_human_resources.calendar_event_members.joined IS 'Дата присоединения к событию.';