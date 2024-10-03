CREATE TABLE IF NOT EXISTS roles.calendar_event_role_members
(
    role_id         BIGINT       NOT NULL,
    event_id        BIGINT       NOT NULL,
    role_name       VARCHAR(150) NOT NULL,
    role_sys_name   VARCHAR(150) NOT NULL,
    event_member_id BIGINT       NOT NULL,
    CONSTRAINT pk_calendar_event_role_members_role_id PRIMARY KEY (role_id),
    CONSTRAINT fk_calendar_events_event_id FOREIGN KEY (event_id) REFERENCES project_management_human_resources.calendar_events (event_id)
);

COMMENT ON TABLE roles.calendar_event_role_members IS 'Таблица участников события календаря.';
COMMENT ON COLUMN roles.calendar_event_role_members.role_id IS 'PK.';
COMMENT ON COLUMN roles.calendar_event_role_members.event_id IS 'Id события.';
COMMENT ON COLUMN roles.calendar_event_role_members.role_name IS 'Название роли.';
COMMENT ON COLUMN roles.calendar_event_role_members.role_sys_name IS 'Системное название роли.';
COMMENT ON COLUMN roles.calendar_event_role_members.event_member_id IS 'Системное название роли.';