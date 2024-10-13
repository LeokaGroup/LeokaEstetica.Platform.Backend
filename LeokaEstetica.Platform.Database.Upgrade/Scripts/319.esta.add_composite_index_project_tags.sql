CREATE INDEX idx_project_tags_composite
ON project_management.project_tags (tag_sys_name, project_id, object_tag_type);