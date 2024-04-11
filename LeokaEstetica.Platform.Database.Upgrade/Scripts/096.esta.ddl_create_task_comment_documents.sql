CREATE TABLE documents.task_comment_documents
(
    comment_document_id BIGINT,
    comment_id          BIGINT    NOT NULL,
    document_id         BIGINT    NOT NULL,
    project_id          BIGINT    NOT NULL,
    created_at          TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by          BIGINT    NOT NULL,
    CONSTRAINT pk_task_comment_documents_comment_document_id PRIMARY KEY (comment_document_id),
    CONSTRAINT fk_user_projects_project_id FOREIGN KEY (project_id) REFERENCES "Projects"."UserProjects" ("ProjectId"),
    CONSTRAINT fk_task_comments_comment_id FOREIGN KEY (comment_id) REFERENCES project_management.task_comments (comment_id),
    CONSTRAINT fk_documents_project_documents_document_id FOREIGN KEY (document_id) REFERENCES documents.project_documents (document_id),
    CONSTRAINT fk_users_created_by FOREIGN KEY (created_by) REFERENCES dbo."Users" ("UserId")
);

COMMENT ON TABLE documents.task_comment_documents IS 'Таблица файлов комментариев задачи.';
COMMENT ON COLUMN documents.task_comment_documents.comment_document_id IS 'PK. Id файла комментария.';
COMMENT ON COLUMN documents.task_comment_documents.document_id IS 'Id документа.';
COMMENT ON COLUMN project_management.task_comments.project_id IS 'Id проекта.';
COMMENT ON COLUMN documents.task_comment_documents.created_at IS 'Дата создания комментария.';
COMMENT ON COLUMN documents.task_comment_documents.created_by IS 'Id пользователя создавшего комментарий.';