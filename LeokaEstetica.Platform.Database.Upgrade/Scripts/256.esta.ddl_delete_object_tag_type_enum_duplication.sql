ALTER TABLE project_management.project_tags
    DROP COLUMN object_tag_type;

ALTER TABLE project_management.project_tags
    ADD COLUMN object_tag_type project_management.object_tag_type_enum;

DROP TYPE public.object_tag_type_enum;