CREATE TYPE object_tag_type_enum AS ENUM ('project', 'project-documentation');

ALTER TABLE project_management.project_tags
    ADD COLUMN object_tag_type object_tag_type_enum;