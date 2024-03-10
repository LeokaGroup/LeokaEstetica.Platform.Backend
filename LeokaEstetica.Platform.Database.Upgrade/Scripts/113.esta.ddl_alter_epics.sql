ALTER TABLE project_management.epics
    ADD COLUMN date_start    TIMESTAMP NULL,
    ADD COLUMN date_end      TIMESTAMP NULL,
    ADD COLUMN priority_id   INT       NOT NULL DEFAULT 2,
    ADD COLUMN tag_ids       INTEGER[] NULL,
    ADD COLUMN resolution_id INT       NULL;

COMMENT ON COLUMN project_management.epics.date_start IS 'Дата начала эпика.';
COMMENT ON COLUMN project_management.epics.date_end IS 'Дата окончания эпика.';
COMMENT ON COLUMN project_management.epics.priority_id IS 'Приоритет эпика.';
COMMENT ON COLUMN project_management.epics.tag_ids IS 'Список Id тегов эпика. В виде Jsonb.';
COMMENT ON COLUMN project_management.epics.resolution_id IS 'Id резолюции.';