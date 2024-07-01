CREATE TABLE project_management.wiki_tree_folder_relations
(
    relation_id BIGSERIAL NOT NULL,
    folder_id   BIGINT    NOT NULL,
    parent_id   BIGINT,
    child_id    BIGINT,
    CONSTRAINT pk_wiki_tree_folder_relations_relation_id PRIMARY KEY (relation_id)
);

COMMENT ON TABLE project_management.wiki_tree_folder_relations IS 'Таблица связей папок wiki проекта компании.';

COMMENT ON COLUMN project_management.wiki_tree_folder_relations.relation_id IS 'PK.';

COMMENT ON COLUMN project_management.wiki_tree_folder_relations.folder_id IS 'Id папки.';

COMMENT ON COLUMN project_management.wiki_tree_folder_relations.parent_id IS 'Id родительской папки.';

COMMENT ON COLUMN project_management.wiki_tree_folder_relations.child_id IS 'Id дочерней папки.';