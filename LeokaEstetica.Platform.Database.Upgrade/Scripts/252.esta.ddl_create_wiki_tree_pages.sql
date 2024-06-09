CREATE TABLE project_management.wiki_tree_pages
(
    page_id          BIGSERIAL                NOT NULL,
    folder_id        BIGINT                   NULL,
    page_name        VARCHAR(200)             NOT NULL,
    page_description TEXT                     NOT NULL,
    wiki_tree_id     BIGINT                   NOT NULL,
    created_by       BIGINT                   NOT NULL,
    created_at       TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT pk_wiki_tree_pages_page_id PRIMARY KEY (page_id),
    CONSTRAINT fk_wiki_tree_wiki_id FOREIGN KEY (wiki_tree_id) REFERENCES project_management.wiki_tree (wiki_tree_id),
    CONSTRAINT fk_users_user_id_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE project_management.wiki_tree_pages IS 'Таблица дерева папок wiki проекта компании.';
COMMENT ON COLUMN project_management.wiki_tree_pages.page_id IS 'PK.';
COMMENT ON COLUMN project_management.wiki_tree_pages.folder_id IS 'Id папки.';
COMMENT ON COLUMN project_management.wiki_tree_pages.page_name IS 'Название страницы.';
COMMENT ON COLUMN project_management.wiki_tree_pages.page_description IS 'Описание страницы.';
COMMENT ON COLUMN project_management.wiki_tree_pages.wiki_tree_id IS 'Id дерева.';
COMMENT ON COLUMN project_management.wiki_tree_pages.created_by IS 'Id пользователя.';
COMMENT ON COLUMN project_management.wiki_tree_pages.created_at IS 'Дата создания страницы.';