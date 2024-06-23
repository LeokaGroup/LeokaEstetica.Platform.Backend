CREATE TABLE project_management.wiki_tree_folders
(
    wiki_tree_folder_id BIGSERIAL
        CONSTRAINT pk_wiki_tree_folders_wiki_tree_folder_id
            PRIMARY KEY,
    folder_id           BIGINT                                 NOT NULL,
    wiki_tree_id        BIGINT                                 NOT NULL
        CONSTRAINT fk_wiki_tree_wiki_id
            REFERENCES project_management.wiki_tree (wiki_tree_id),
    folder_name         VARCHAR(200)                           NOT NULL,
    created_by          BIGINT                                 NOT NULL
        CONSTRAINT fk_users_user_id_created_by
            REFERENCES dbo."Users",
    created_at          TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL
);

COMMENT ON TABLE project_management.wiki_tree_folders IS 'Таблица дерева папок wiki проекта компании.';

COMMENT ON COLUMN project_management.wiki_tree_folders.folder_id IS 'PK.';

COMMENT ON COLUMN project_management.wiki_tree_folders.wiki_tree_id IS 'Id дерева.';

COMMENT ON COLUMN project_management.wiki_tree_folders.folder_name IS 'Название папки.';

COMMENT ON COLUMN project_management.wiki_tree_folders.created_by IS 'Id пользователя.';

COMMENT ON COLUMN project_management.wiki_tree_folders.created_at IS 'Дата создания страницы.';