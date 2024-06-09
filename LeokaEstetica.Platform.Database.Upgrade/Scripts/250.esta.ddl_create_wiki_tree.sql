CREATE TABLE project_management.wiki_tree
(
    wiki_id      BIGSERIAL NOT NULL,
    wiki_tree_id BIGINT    NOT NULL,
    project_id   BIGINT    NOT NULL,
    CONSTRAINT pk_wiki_tree_wiki_id PRIMARY KEY (wiki_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId")
);

COMMENT ON TABLE project_management.wiki_tree IS 'Таблица дерева wiki проекта компании.';
COMMENT ON COLUMN project_management.wiki_tree.wiki_id IS 'PK.';
COMMENT ON COLUMN project_management.wiki_tree.wiki_tree_id IS 'Составной индекс.';
COMMENT ON COLUMN project_management.wiki_tree.project_id IS 'Id проекта. Составной индекс.';

CREATE UNIQUE INDEX wiki_tree_wiki_tree_id_idx ON project_management.wiki_tree (wiki_tree_id);
CREATE UNIQUE INDEX wiki_tree_wiki_wiki_tree_id_project_id_idx ON project_management.wiki_tree (wiki_tree_id, project_id);