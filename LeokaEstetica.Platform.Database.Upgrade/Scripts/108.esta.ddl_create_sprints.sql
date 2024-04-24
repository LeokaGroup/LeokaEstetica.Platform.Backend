CREATE TABLE project_management.sprints
(
    sprint_id        BIGSERIAL,
    date_start       TIMESTAMP NULL,
    date_end         TIMESTAMP NULL,
    sprint_goal      TEXT      NULL,
    sprint_status_id INT       NOT NULL,
    project_id       BIGINT    NOT NULL,
    CONSTRAINT pk_sprints_sprint_id PRIMARY KEY (sprint_id),
    CONSTRAINT fk_projects_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId")
);

COMMENT ON TABLE project_management.sprints IS 'Таблица спринтов.';
COMMENT ON COLUMN project_management.sprints.sprint_id IS 'PK.';
COMMENT ON COLUMN project_management.sprints.date_start IS 'Дата начала спринта.';
COMMENT ON COLUMN project_management.sprints.date_end IS 'Дата окончания спринта.';
COMMENT ON COLUMN project_management.sprints.sprint_goal IS 'Цель спринта.';
COMMENT ON COLUMN project_management.sprints.sprint_status_id IS 'Id статуса спринта.';
COMMENT ON COLUMN project_management.sprints.project_id IS 'Id проекта, которому принадлежит спринт.';