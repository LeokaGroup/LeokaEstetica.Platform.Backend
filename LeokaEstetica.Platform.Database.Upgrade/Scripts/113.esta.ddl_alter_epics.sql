ALTER TABLE project_management.epics
    ADD COLUMN IF NOT EXISTS date_start    TIMESTAMP NULL,
    ADD COLUMN IF NOT EXISTS date_end      TIMESTAMP NULL,
    ADD COLUMN IF NOT EXISTS priority_id   INT       NOT NULL DEFAULT 2,
    ADD COLUMN IF NOT EXISTS tag_ids       INTEGER[] NULL,
    ADD COLUMN IF NOT EXISTS resolution_id INT       NULL;

COMMENT ON COLUMN project_management.epics.date_start IS 'Дата начала эпика.';
COMMENT ON COLUMN project_management.epics.date_end IS 'Дата окончания эпика.';
COMMENT ON COLUMN project_management.epics.priority_id IS 'Приоритет эпика.';
COMMENT ON COLUMN project_management.epics.tag_ids IS 'Список Id тегов эпика. В виде Jsonb.';
COMMENT ON COLUMN project_management.epics.resolution_id IS 'Id резолюции.';