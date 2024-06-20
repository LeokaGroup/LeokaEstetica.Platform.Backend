CREATE TABLE project_management.wiki_tree_folders
(
    folder_id    BIGSERIAL                NOT NULL,
    wiki_tree_id BIGINT                   NOT NULL,
    folder_name  VARCHAR(200)             NOT NULL,
    parent_id    BIGINT                   NULL,
    child_id     BIGINT                   NULL,
    created_by   BIGINT                   NOT NULL,
    created_at   TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_wiki_tree_folders_folder_id PRIMARY KEY (folder_id),
    CONSTRAINT fk_wiki_tree_wiki_id FOREIGN KEY (wiki_tree_id) REFERENCES project_management.wiki_tree (wiki_tree_id),
    CONSTRAINT fk_users_user_id_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE project_management.wiki_tree_folders IS 'Таблица дерева папок wiki проекта компании.';
COMMENT ON COLUMN project_management.wiki_tree_folders.folder_id IS 'PK.';
COMMENT ON COLUMN project_management.wiki_tree_folders.wiki_tree_id IS 'Id дерева.';
COMMENT ON COLUMN project_management.wiki_tree_folders.folder_name IS 'Название папки.';
COMMENT ON COLUMN project_management.wiki_tree_folders.parent_id IS 'Id родительской папки.';
COMMENT ON COLUMN project_management.wiki_tree_folders.child_id IS 'Id дочерней папки.';
COMMENT ON COLUMN project_management.wiki_tree_folders.created_by IS 'Id пользователя.';
COMMENT ON COLUMN project_management.wiki_tree_folders.created_at IS 'Дата создания страницы.';