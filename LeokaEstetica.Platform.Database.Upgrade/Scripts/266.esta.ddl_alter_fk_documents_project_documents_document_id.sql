ALTER TABLE documents.task_comment_documents
DROP CONSTRAINT fk_documents_project_documents_document_id;

ALTER TABLE documents.task_comment_documents
    ADD CONSTRAINT fk_documents_project_documents_document_id FOREIGN KEY (document_id) REFERENCES documents.project_documents (document_id) ON DELETE CASCADE;