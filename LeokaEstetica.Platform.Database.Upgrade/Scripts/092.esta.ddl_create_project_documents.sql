CREATE TABLE documents.project_documents
(
    document_id        BIGSERIAL,
    document_type      documents.document_type_enum,
    document_name      VARCHAR(255) NOT NULL,
    document_extension VARCHAR(10)  NOT NULL,
    created            TIMESTAMP    NOT NULL DEFAULT NOW(),
    updated            TIMESTAMP    NOT NULL DEFAULT NOW(),
    project_id         BIGINT       NOT NULL,
    task_id            BIGINT       NOT NULL,
    CONSTRAINT pk_project_documents_document_id PRIMARY KEY (document_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId"),
    CONSTRAINT fk_user_projects_task_id FOREIGN KEY (task_id) REFERENCES project_management.project_tasks (task_id)
);

COMMENT ON TABLE documents.project_documents IS 'Таблица документов.';
COMMENT ON COLUMN documents.project_documents.document_id IS 'PK. Id документа';
COMMENT ON COLUMN documents.project_documents.document_type IS 'Тип документа.';
COMMENT ON COLUMN documents.project_documents.document_name IS 'Название документа.';
COMMENT ON COLUMN documents.project_documents.document_extension IS 'Расширение документа.';
COMMENT ON COLUMN documents.project_documents.created IS 'Дата создания документа.';
COMMENT ON COLUMN documents.project_documents.updated IS 'Дата обновления документа.';
COMMENT ON COLUMN documents.project_documents.project_id IS 'Id проекта.';
COMMENT ON COLUMN documents.project_documents.task_id IS 'Id задачи.';